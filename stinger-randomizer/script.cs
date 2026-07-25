using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

/*
 * Description: This action will randomly select a stinger transition from your created transitions list
 * Note: Use the OBS scene changed trigger to use this action. Place a short delay action before this action if necessary.
 */

public class CPHInline {
    public class TransitionListResponse {
        public string currentSceneTransitionName { get; set; }
        public string currentSceneTransitionUuid { get; set; }
        public string currentSceneTransitionKind { get; set; }
        public List<TransitionItem> transitions { get; set; }
    }

    public class TransitionItem {
        public string transitionName { get; set; }
        public string transitionUuid { get; set; }
        public string transitionKind { get; set; }
        public bool transitionFixed { get; set; }
        public bool transitionConfigurable { get; set; }
    }

    public class TransitionChange {
        public string transitionName { get; set; }
    }

    public class TransitionCursorResponse {
        public double transitionCursor { get; set; }
    }
    
    public bool Execute() {
        string cursorResponse = CPH.ObsSendRaw("GetCurrentSceneTransitionCursor", "{}");
        if (!string.IsNullOrWhiteSpace(cursorResponse)) {
            var cursorData = JsonConvert.DeserializeObject<TransitionCursorResponse>(cursorResponse);
            if (cursorData != null && cursorData.transitionCursor < 1.0) {
                CPH.LogInfo($"Transition currently in progress (cursor: {cursorData.transitionCursor:F3}) — skipping change");
                return false;
            }
        }
        
        string obsResponse = CPH.ObsSendRaw("GetSceneTransitionList", "{}");
        if (string.IsNullOrWhiteSpace(obsResponse)) {
            CPH.LogWarn("No response data received");
            return false;
        }
        
        var data = JsonConvert.DeserializeObject<TransitionListResponse>(obsResponse);
        if (data == null) {
            CPH.LogWarn("Could not parse response");
            return false;
        }
        if (data.transitions == null) {
            CPH.LogWarn("No transitions found");
            return false;
        }

        string currentTransitionName = data.currentSceneTransitionName;

        List<TransitionItem> availableTransitions = data.transitions
            .Where(t => t.transitionKind == "obs_stinger_transition" && t.transitionName != currentTransitionName)
            .ToList();
        
        if (availableTransitions.Count == 0) {
            CPH.LogWarn("No alternative transitions found");
            return false;
        }

        Random rand = new Random();
        int randomIndex = rand.Next(availableTransitions.Count);
        var targetTransition = availableTransitions[randomIndex];

        CPH.LogInfo($"Current: {currentTransitionName} | Randomly Selected New: {targetTransition.transitionName}");

        var transitionChangeRequest = new TransitionChange() {
            transitionName = targetTransition.transitionName
        };
        var transitionChangeRequestJson = JsonConvert.SerializeObject(transitionChangeRequest);
        CPH.ObsSendRaw("SetCurrentSceneTransition", transitionChangeRequestJson);
        return true;
    }
}

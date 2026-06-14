using System;
using Newtonsoft.Json;

/*
 * Description: This action is to be used with LiveVisionKit (https://github.com/Crowsinc/LiveVisionKit)
 * to enable or disable the stabilization filter on a specific source during an IRL stream.
 */

public class CPHInline {
    public void Init() {
        bool stabilizerEnabled = false;

        try {
            stabilizerEnabled = CPH.GetGlobalVar<bool>("stabilizerEnabled");
        } catch {
            CPH.SetGlobalVar("stabilizerEnabled", false);
        }
    }

    private bool SetFilterState(string sourceName, bool filterState) {
        CPH.ObsSendRaw(JsonConvert.SerializeObject(new {
            sourceName = sourceName,
            filterName = "(LVK) Video Stabilizer",
            filterSettings = new {
                STAB_DISABLED = filterState
            }
        }));
        return true;
    }

    public bool Execute() {
        bool currentValue = CPH.GetGlobalVar<bool>("stabilizerEnabled");
        bool newValue = !currentValue;

        bool filterStateChanged = SetFilterState("SLS-SRTLA-BELABOX.live", newValue);
        if (filterStateChanged) {
            CPH.SetGlobalVar("stabilizerEnabled", newValue);
        }

        return true;
    }
}

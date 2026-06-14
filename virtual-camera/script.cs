using Newtonsoft.Json;

/*
 * Description: This action serves to make sure that the Virtual Camera functionality is available when needed
 * Note: Look into starting Virtual Camera when OBS opens if necessary
 */

public class VirtualCamStatusResponse {
    public bool outputActive { get; set; }
}

public class CPHInline {
    public void GetVirtualCamStatus(out bool isVirtualCamActive) {
        string responseString = CPH.ObsSendRaw("GetVirtualCamStatus", "");
        VirtualCamStatusResponse data = JsonConvert.DeserializeObject<VirtualCamStatusResponse>(responseString);
        isVirtualCamActive = data.outputActive;
    }

    public bool Execute() {
        if (!CPH.ObsIsConnected()) {
            CPH.SendMessage("Error: OBS is not connected");
            return false;
        }
        
        this.GetVirtualCamStatus(out bool isVirtualCamActive);
        if (!isVirtualCamActive) {
            CPH.ObsSendRaw("StartVirtualCam", "");
            CPH.SendMessage("Virtual cam started");
        } else {
            CPH.SendMessage("Virtual cam is already running");
        }
        
        return true;
    }
}

using Newtonsoft.Json;

public class VirtualCamStatusResponse {
    public bool outputActive { get; set; }
}

public class CPHInline {
    public void GetVirtualCamStatus(out bool isVirtualCameraActive) {
        string responseString = CPH.ObsSendRaw("GetVirtualCamStatus", "");
        VirtualCamStatusResponse data = JsonConvert.DeserializeObject<VirtualCamStatusResponse>(responseString);
        isVirtualCameraActive = data.outputActive;
    }

    public bool Execute() {
        if (!CPH.ObsIsConnected()) {
            CPH.SendMessage("Error: OBS is not connected");
            return false;
        }
        
        this.GetVirtualCamStatus(out bool isVirtualCameraActive);
        if (!isVirtualCameraActive) {
            CPH.ObsSendRaw("StartVirtualCam", "");
            CPH.SendMessage("Virtual camera started");
        } else {
            CPH.SendMessage("Error: Could not start virtual camera");
        }
        
        return true;
    }
}

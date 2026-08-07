using System;

/*
 * Description: This action is meant to be used to pin an advert message in chat on certain events
 */

public class CPHInline {
    public bool Execute() {
        var message = "FIND OUT MORE @ https://docs.streamer.bot";
        CPH.TwitchSendAndPinMessage(message);
        return true;
    }
}

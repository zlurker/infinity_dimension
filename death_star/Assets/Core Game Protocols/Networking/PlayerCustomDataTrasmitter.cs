using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    public override void MessageRecievedCallback() {
        Debug.LogFormat("Info came in at:{0}", new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds());
    }

    public void SendFiles() {

        int[] datafilesToSend = new int[] { 0, 1, 3, 4, 5, 6 };

        byte[][][] cData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(datafilesToSend);

        for (int i=0; i < cData.Length; i++)
            for (int j=0; j < cData[i].Length; j++) {
                bytesToSend = cData[i][j];
                SendEncodedMessages();
            }
    }

    public void SendAbilityData(string text) {

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum ServerSideMethods {
    CLIENT_IDENTITY, START_GAME
}

public class ServerChannel : NetworkMessageEncoder {

    public override void MessageRecievedCallback() {

        int actionType = BitConverter.ToInt32(bytesRecieved, 0);

        switch((ServerSideMethods)actionType) {
            case ServerSideMethods.CLIENT_IDENTITY:
                ClientProgram.clientId = targetId;
                break;
            case ServerSideMethods.START_GAME:
                break;
        }
    }

    public void CommunicateServerSide(ServerSideMethods mode) {
        bytesToSend = BitConverter.GetBytes((int)mode);
        SendEncodedMessages();
    }
}

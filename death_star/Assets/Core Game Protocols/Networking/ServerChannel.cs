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
        Debug.Log(actionType);

        switch((ServerSideMethods)actionType) {
            case ServerSideMethods.CLIENT_IDENTITY:
                ClientProgram.clientId = targetId;
                break;
            case ServerSideMethods.START_GAME:
                if(LobbyScript.lobbyInst != null) {
                    LoadedData.startTimeSinceConnection = BitConverter.ToDouble(bytesRecieved, 4);
                    Debug.Log("realtime: " +Time.realtimeSinceStartup);
                    Debug.Log("connection time: "+ LoadedData.connectionTimeOffset);
                    Debug.Log("starttime since conn: " + LoadedData.startTimeSinceConnection);
                    LobbyScript.lobbyInst.OnStartSignal();
                }
                break;
        }
    }

    public void CommunicateServerSide(ServerSideMethods mode) {
        Debug.Log("Start!");
        bytesToSend = BitConverter.GetBytes((int)mode);
        SendEncodedMessages();
    }
}

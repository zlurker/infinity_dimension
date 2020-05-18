using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum ServerSideMethods {
    CLIENT_IDENTITY, START_GAME, HOST, HOST_SELECT
}

public class ServerChannel : NetworkMessageEncoder {

    public override void MessageRecievedCallback() {

        int actionType = BitConverter.ToInt32(bytesRecieved, 0);
        //Debug.Log(actionType);

        switch((ServerSideMethods)actionType) {
            case ServerSideMethods.CLIENT_IDENTITY:
                ClientProgram.clientId = targetId;
                break;

            case ServerSideMethods.START_GAME:
                if(LobbyScript.lobbyInst != null) {
                    bytesToSend = BitConverter.GetBytes((int)ServerSideMethods.HOST);
                    SendEncodedMessages();

                    LoadedData.startTimeSinceConnection = BitConverter.ToDouble(bytesRecieved, 4);
                    Debug.Log("realtime: " + Time.realtimeSinceStartup);
                    Debug.Log("connection time: " + LoadedData.connectionTimeOffset);
                    Debug.Log("starttime since conn: " + LoadedData.startTimeSinceConnection);
                    LobbyScript.lobbyInst.OnStartSignal();
                }
                break;

            case ServerSideMethods.HOST:
                //ClientProgram.hostId = ClientProgram.clientId;
                bytesToSend = BitConverter.GetBytes(actionType);
                SendEncodedMessages();
                break;

            case ServerSideMethods.HOST_SELECT:
                ClientProgram.hostId = BitConverter.ToInt32(bytesRecieved, 4);
                Debug.LogWarningFormat("Current ID: {0}, Host selected: {1}",ClientProgram.clientId, BitConverter.ToInt32(bytesRecieved, 4));
                break;
        }
    }

    public void CommunicateServerSide(ServerSideMethods mode) {
        Debug.Log("Start!");
        bytesToSend = BitConverter.GetBytes((int)mode);
        SendEncodedMessages();
    }
}

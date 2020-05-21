using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSignalEncoder : NetworkMessageEncoder {

    public void SendInputSignal(int playerId, string abilityName) {
        List<byte> inputData = new List<byte>();

        if(ClientProgram.clientId != ClientProgram.hostId) {
            inputData.AddRange(BitConverter.GetBytes(playerId));
            inputData.AddRange(Encoding.Default.GetBytes(abilityName));
            SetBytesToSend(inputData.ToArray());
            return;
        }

        AbilitiesManager.aData[playerId].abilties[abilityName].CreateAbilityNetworkData();
    }

    public override void MessageRecievedCallback() {

        if(ClientProgram.clientId == ClientProgram.hostId) {
            int playerId = BitConverter.ToInt32(bytesRecieved, 0);
            string abilityName = Encoding.Default.GetString(bytesRecieved, 4, bytesRecieved.Length - 4);
            AbilitiesManager.aData[playerId].abilties[abilityName].CreateAbilityNetworkData();
        }
    }

}

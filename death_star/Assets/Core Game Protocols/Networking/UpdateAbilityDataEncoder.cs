using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class UpdateAbilityDataEncoder : NetworkMessageEncoder {

    public void SendUpdatedNodeData<T>(int central, int instId, int ability, int var, T value) {

        byte[] vBytes = new byte[0];
        int argType = -1;

        if(value is int) {
            vBytes = BitConverter.GetBytes((int)(object)value);
            argType = 0;
        }

        if(value is float) {
            vBytes = BitConverter.GetBytes((float)(object)value);
            argType = 1;
        }

        if(value is string) {
            vBytes = Encoding.UTF8.GetBytes((string)(object)value);
            argType = 2;
        }

        if(argType > -1) {
            bytesToSend = new byte[20 + vBytes.Length];

            byte[] cBytes = BitConverter.GetBytes(central);
            byte[] iBytes = BitConverter.GetBytes(instId);
            byte[] aBytes = BitConverter.GetBytes(ability);
            byte[] vaBytes = BitConverter.GetBytes(var);
            byte[] argBytes = BitConverter.GetBytes(argType);

            Buffer.BlockCopy(cBytes, 0, bytesToSend, 0, 4);
            Buffer.BlockCopy(iBytes, 0, bytesToSend, 4, 4);
            Buffer.BlockCopy(aBytes, 0, bytesToSend, 8, 4);
            Buffer.BlockCopy(vaBytes, 0, bytesToSend, 12, 4);
            Buffer.BlockCopy(argBytes, 0, bytesToSend, 16, 4);
            Buffer.BlockCopy(vBytes, 0, bytesToSend, 20, vBytes.Length);

            SendEncodedMessages();
        }
    }

    public override void MessageRecievedCallback() {
        int central = BitConverter.ToInt32(bytesRecieved, 0);
        int instId = BitConverter.ToInt32(bytesRecieved, 4);

        // Checks if inst id given was updated.
        if(NetworkObjectTracker.inst.CheckIfInstIdMatches(central, instId)) {

            int ability = BitConverter.ToInt32(bytesRecieved, 8);
            int var = BitConverter.ToInt32(bytesRecieved, 12);
            int argType = BitConverter.ToInt32(bytesRecieved, 16);

            // Do checks for inst id.
            switch(argType) {
                case 0: //int
                    int iData = BitConverter.ToInt32(bytesRecieved, 20);
                    Debug.Log(iData);
                    break;

                case 1: //float
                    float fData = BitConverter.ToInt32(bytesRecieved, 20);
                    break;

                case 2: //string
                    string sData = Encoding.Default.GetString(bytesRecieved, 16, bytesRecieved.Length - 20);
                    break;
            }
        }
    }
}

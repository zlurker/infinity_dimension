using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkEncoderTypes {
    ABILITY_INPUT
}

public class NetworkMessageEncoder {

    public static List<NetworkMessageEncoder> encoders;

    private int encoderId;
    protected byte[] bytesToSend;

    public NetworkMessageEncoder(int id) {
        encoderId = id;
    }
	
    public void SendEncodedMessages() {
        byte[] eT = BitConverter.GetBytes(encoderId);
        byte[] nwMsg = new byte[bytesToSend.Length + 4];

        Buffer.BlockCopy(eT, 0, nwMsg, 0, 4);
        Buffer.BlockCopy(bytesToSend, 0, nwMsg, 4, bytesToSend.Length);
        ClientProgram.clientInst.AddNetworkMessage(nwMsg);        
    }

    public virtual void RecieveEncodedMessages(byte[] msg) {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessageEncoder {

    public static NetworkMessageEncoder[] encoders;

    private int encoderId;
    protected byte[] bytesToSend;
    protected byte[] bytesRecieved;

    public void SetEncoderId(int id) {
        encoderId = id;
    }
	
    public void SendEncodedMessages() {
        byte[] eT = BitConverter.GetBytes(encoderId);
        byte[] nwMsg = new byte[bytesToSend.Length + 4];

        Buffer.BlockCopy(eT, 0, nwMsg, 0, 4);
        Buffer.BlockCopy(bytesToSend, 0, nwMsg, 4, bytesToSend.Length);
        ClientProgram.clientInst.AddNetworkMessage(nwMsg);        
    }

    public void RecieveEncodedMessages(byte[] msg) {
        bytesRecieved = new byte[msg.Length - 4];
        Buffer.BlockCopy(msg, 4, bytesRecieved, 0, msg.Length - 4);
        MessageRecievedCallback();
    }

    public virtual void MessageRecievedCallback() {
        
    }
}

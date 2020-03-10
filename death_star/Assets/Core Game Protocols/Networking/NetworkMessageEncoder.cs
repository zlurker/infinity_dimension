using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkEncoderTypes {
    SERVER_CHANNEL, ABILITY_INPUT, UPDATE_ABILITY_DATA, CUSTOM_DATA_TRASMIT, IMAGE_DATA_TRANSMIT
}

public class NetworkMessageEncoder {

    public static NetworkMessageEncoder[] encoders;

    private int encoderId;

    protected int targetId;
    protected byte[] bytesToSend;
    protected byte[] bytesRecieved;
    
    public void SetEncoderId(int id) {
        encoderId = id;
    }

    public void SetBytesToSend(byte[] bytes) {
        bytesToSend = bytes;
        SendEncodedMessages();
    }

    public void SendEncodedMessages() {
        byte[] eT = BitConverter.GetBytes(encoderId);
        byte[] cId = BitConverter.GetBytes(ClientProgram.clientId);
        byte[] nwMsg = new byte[bytesToSend.Length + 8];

        Buffer.BlockCopy(eT, 0, nwMsg, 0, 4);
        Buffer.BlockCopy(cId, 0, nwMsg, 4, 4);
        Buffer.BlockCopy(bytesToSend, 0, nwMsg, 8, bytesToSend.Length);

        if(ClientProgram.clientInst != null)
            ClientProgram.clientInst.AddNetworkMessage(nwMsg);
    }

    public static void SortEncodedMessages(byte[] msg) {
        int encoder = BitConverter.ToInt32(msg, 0);
        encoders[encoder].RecieveEncodedMessages(msg);
    }

    public void RecieveEncodedMessages(byte[] msg) {
        bytesRecieved = new byte[msg.Length - 8];
        Buffer.BlockCopy(msg, 8, bytesRecieved, 0, msg.Length - 8);

        targetId = BitConverter.ToInt32(msg, 4);
        MessageRecievedCallback();
    }

    public virtual void MessageRecievedCallback() {

    }
}

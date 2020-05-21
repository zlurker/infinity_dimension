using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkEncoderTypes {
    SERVER_CHANNEL,
    UPDATE_ABILITY_DATA,
    CUSTOM_DATA_TRASMIT,
    IMAGE_DATA_TRANSMIT,
    CHRACTER_CREATION,
    MANIFEST,
    INPUT_SIGNAL
}

public class NetworkMessageEncoder {

    public static NetworkMessageEncoder[] encoders;

    private int encoderId;

    protected int targetId;
    protected double timestamp;
    protected byte[] bytesToSend;
    protected byte[] bytesRecieved;
    
    public virtual void CalibrateEncoder(int id) {
        encoderId = id;
    }

    public void SetBytesToSend(byte[] bytes) {
        bytesToSend = bytes;
        SendEncodedMessages();
    }

    public void SendEncodedMessages() {
        byte[] eT = BitConverter.GetBytes(encoderId);
        byte[] cId = BitConverter.GetBytes(ClientProgram.clientId);
        byte[] tS = BitConverter.GetBytes(LoadedData.GetCurrentTimestamp());
        byte[] nwMsg = new byte[bytesToSend.Length + 16];

        Buffer.BlockCopy(eT, 0, nwMsg, 0, 4);
        Buffer.BlockCopy(cId, 0, nwMsg, 4, 4);
        Buffer.BlockCopy(tS, 0, nwMsg, 8, 8);
        Buffer.BlockCopy(bytesToSend, 0, nwMsg, 16, bytesToSend.Length);

        if(ClientProgram.clientInst != null)
            ClientProgram.clientInst.AddNetworkMessage(nwMsg);
    }

    public static void SortEncodedMessages(byte[] msg) {
        int encoder = BitConverter.ToInt32(msg, 0);
        encoders[encoder].RecieveEncodedMessages(msg);
    }

    public void RecieveEncodedMessages(byte[] msg) {
        bytesRecieved = new byte[msg.Length - 16];
        Buffer.BlockCopy(msg, 16, bytesRecieved, 0, msg.Length - 16);

        targetId = BitConverter.ToInt32(msg, 4);
        timestamp = BitConverter.ToDouble(msg, 8);

        //Debug.Log("Timestamp:" + timestamp);
        MessageRecievedCallback();
    }

    public virtual void MessageRecievedCallback() {

    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO.Compression;

public enum NetworkCommandType {
    CREATE_NETOBJECT, UPDATE_NETOBJECT, DELETE_NETOBJECT
}

public enum NetworkObjectType {
    CENTRAL_THREADS
}

public class ClientProgram : MonoBehaviour {

    public static ClientProgram clientInst;
    public static int clientId = -1;
    public static int hostId = -1;

    private List<byte> incoming;
    private List<byte[]> outgoing;

    private float prevMsgTimer;

    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    private byte[] _sendBuffer = new byte[8142];
    private int currMsgLength;

    void Start() {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        outgoing = new List<byte[]>();
        incoming = new List<byte>();

        currMsgLength = -1;

        clientSock.Connect("178.128.95.63", 5000);
        LoadedData.connectionTimeOffset = Time.realtimeSinceStartup;

        DontDestroyOnLoad(this);
        clientInst = this;

        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);

    }

    void OnRecieve(IAsyncResult asyncResult) {

        int recieved = clientSock.EndReceive(asyncResult);

        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        lock(incoming)
            incoming.AddRange(recData);

        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

    // Update is called once per frame
    void Update() {
        lock(incoming)
            ParseCommands();

        /*// Keep alive if client is host.
        if(clientId == hostId)
            if(outgoing.Count == 0)
                if(Time.realtimeSinceStartup - prevMsgTimer > 0.1f)
                    AddNetworkMessage(new byte[0]);*/

        if(outgoing.Count > 0) {
            clientSock.BeginSend(outgoing[0], 0, outgoing[0].Length, SocketFlags.None, null, null);
            outgoing.RemoveAt(0);
            prevMsgTimer = Time.realtimeSinceStartup;
        }
    }

    public void ParseCommands() {

        if(incoming.Count > 4) {

            if(currMsgLength == -1)
                currMsgLength = BitConverter.ToInt32(incoming.ToArray(), 0);

            if(incoming.Count >= currMsgLength) {

                if(currMsgLength > 4)
                    NetworkMessageEncoder.SortEncodedMessages(incoming.GetRange(4, currMsgLength - 4).ToArray());

                incoming.RemoveRange(0, currMsgLength);
                currMsgLength = -1;
                ParseCommands();
            }
        }
    }

    public void AddNetworkMessage(byte[] message) {
        int msgLen = message.Length + 4;

        byte[] lenBytes = BitConverter.GetBytes(msgLen);
        byte[] processedMsg = new byte[msgLen];

        Buffer.BlockCopy(lenBytes, 0, processedMsg, 0, 4);
        Buffer.BlockCopy(message, 0, processedMsg, 4, message.Length);
        outgoing.Add(processedMsg);
    }
}

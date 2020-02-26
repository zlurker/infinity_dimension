using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class ClientProgram : MonoBehaviour {

    public static ClientProgram clientInst;

    private List<byte> incoming;
    private List<byte[]> outgoing;

    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    private byte[] _sendBuffer = new byte[8142];

    void Start() {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        outgoing = new List<byte[]>();
        incoming = new List<byte>();
        DontDestroyOnLoad(this);
        clientInst = this;

        clientSock.Connect("178.128.95.63", 5000);

        AddNetworkMessage(Encoding.UTF8.GetBytes("LOLOLOL hahaah okay gtg you little scums bye."));

        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

    void OnSent(IAsyncResult asyncResult) {
        Debug.Log("Msg Sent");
    }

    void OnRecieve(IAsyncResult asyncResult) {
        //Debug.Log("Recieved!");
        int recieved = clientSock.EndReceive(asyncResult);

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        //Debug.Log(Encoding.Default.GetString(recData, 0, recData.Length));

        //Start receiving again

        incoming.AddRange(recData);
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

    // Update is called once per frame
    void Update() {

        ParseCommands();

        if(outgoing.Count > 0) {
            clientSock.BeginSend(outgoing[0], 0, outgoing[0].Length, SocketFlags.None, new AsyncCallback(OnSent), null);
            outgoing.RemoveAt(0);
        }
    }

    public void ParseCommands() {
        if(incoming.Count > 4) {
            int currMsgLength = BitConverter.ToInt32( incoming.GetRange(0,4).ToArray(),0);

            if (incoming.Count >= currMsgLength) {
                HandleCommand(incoming.GetRange(4, currMsgLength - 4).ToArray());
                incoming.RemoveRange(0, currMsgLength);
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

    public void HandleCommand(byte[] command) {
        Debug.Log(command.Length);
    }
}

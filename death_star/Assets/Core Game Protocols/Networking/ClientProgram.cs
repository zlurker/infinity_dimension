using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class ClientProgram : MonoBehaviour {

    public static ClientProgram clientInst;

    // Network commands
    public const string HOST_CHANNEL = "0";
    public const string IS_NOT_HOST = "0";
    public const string IS_HOST = "1";

    public const string SYNC_CHANNEL = "1";
    public const string SYNC_NODE_DATA = "10";

    public const string ASYNC_CHANNEL = "2";
    public const string ASYNC_INPUT = "20";

    // Network related parsing.
    string splitCommands;

    private bool playerIsHost = false;

    private List<string> incoming;
    private List<string> outgoing;
    //private List<string> host_pirority_outgoing;


    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    private byte[] _sendBuffer = new byte[8142];

    void Start() {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        outgoing = new List<string>();
        incoming = new List<string>();
        DontDestroyOnLoad(this);
        clientInst = this;

        clientSock.Connect("178.128.95.63", 5000);

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

        incoming.Add(Encoding.Default.GetString(recData, 0, recData.Length));
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

    // Update is called once per frame
    void Update() {
        if(incoming.Count > 0) {
            ParseCommands(incoming[0]);
            incoming.RemoveAt(0);
        }

        /*if(host_pirority_outgoing.Count > 0 && playerIsHost) {
            host_pirority_outgoing[0].Replace("\n", string.Empty);
            SendMessage(host_pirority_outgoing[0]);
            outgoing.RemoveAt(0);
        }*/

        if(outgoing.Count > 0) {
            outgoing[0].Replace("\n", string.Empty);
            SendMessages(outgoing[0]);
            outgoing.RemoveAt(0);
        }
    }

    void SendMessages(string msg) {
        Debug.Log("sending" + msg + '\n');
        _sendBuffer = Encoding.GetEncoding("UTF-8").GetBytes(msg + '\n');
        clientSock.BeginSend(_sendBuffer, 0, _sendBuffer.Length, SocketFlags.None, new AsyncCallback(OnSent), null);
    }

    public void AddNetworkMessage(string channelType, string msg) {
        outgoing.Add(channelType + '/' + msg);
    }

    public void AddPirorityNetworkMessage(string channelType, string msg) {
        if(clientSock.Available == 0) {
            Debug.Log("Checking pirority...");
            outgoing.Add(HOST_CHANNEL);
            outgoing.Add(channelType + '/' + msg);
        }
    }

    public void ParseCommands(string command) {

        string[] commands = HandleIncomingCommands(command);

        for(int i = 0; i < commands.Length; i++) {

            string[] commandDir = commands[i].Split(new char[] { '/' }, 2);
            Debug.Log(commandDir[0]);

            switch(commandDir[0]) {

                /*case HOST_CHANNEL:
                    switch(commandDir[1]) {
                        case IS_HOST:
                            //playerIsHost = true;
                            break;
                        case IS_NOT_HOST:
                            //playerIsHost = false;
                            //host_pirority_outgoing.RemoveRange(0, host_pirority_outgoing.Count);
                            break;
                    }
                    break;*/

                case SYNC_CHANNEL:
                    break;

                case SYNC_NODE_DATA:
                    Debug.Log("Supposed to recieve node data here.");
                    break;

                case ASYNC_CHANNEL:
                    break;

                case ASYNC_INPUT:
                    AbilitiesManager.aData[int.Parse(commandDir[1].ToString())].CreateAbility(null);
                    break;
            }
        }
    }

    public string[] HandleIncomingCommands(string command) {
        if(!command.Contains("\n")) {
            splitCommands = command;
            return new string[0];
        }

        bool keepLastCommand = false;
        keepLastCommand = command[command.Length - 1] != '\n' ? true : false;

        string[] commands = command.Split('\n');
        commands[0] = splitCommands + commands[0];
        splitCommands = keepLastCommand ? commands[commands.Length - 1] : "";
        return commands;
    }
}

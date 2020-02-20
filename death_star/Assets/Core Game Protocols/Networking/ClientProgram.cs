using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class ClientProgram : MonoBehaviour {

    //public string toSend;
    //public bool send;

    public static ClientProgram clientInst;
    public string networkStream;

    private List<string> outgoing;
    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    private byte[] _sendBuffer = new byte[8142];

    private CommandCentral cc;
    

    private string msg;

    void Start () {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        cc = new CommandCentral();

        outgoing = new List<string>();
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

        networkStream += Encoding.Default.GetString(recData, 0, recData.Length);
        //Start receiving again
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }
	
	// Update is called once per frame
	void Update () {

        /*if(send) {
            AddNetworkMessage(toSend);
            toSend = "";
            send = false;
        }*/

        int currNetworkStreamCount = networkStream.Length;

        Debug.Log(currNetworkStreamCount);
        if (currNetworkStreamCount > 0) {
            cc.ParseCommands(networkStream.Substring(0, currNetworkStreamCount));
            networkStream = networkStream.Substring(currNetworkStreamCount - 1);
        }

		if (outgoing.Count >0) {
             outgoing[outgoing.Count - 1].Replace("\n", string.Empty);
            _sendBuffer = Encoding.GetEncoding("UTF-8").GetBytes(outgoing[outgoing.Count -1] + '\n');
            clientSock.BeginSend(_sendBuffer, 0, _sendBuffer.Length, SocketFlags.None, new AsyncCallback(OnSent), null);
            outgoing.RemoveAt(outgoing.Count - 1);
        }
	}

    public void AddNetworkMessage(string msg) {
        outgoing.Insert(0, msg);
    }
}

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

    private List<string> incoming;
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
	void Update () {

        if (incoming.Count > 0) {
            cc.ParseCommands(incoming[0]);
            incoming.RemoveAt(0);
        }

		if (outgoing.Count >0) {
             outgoing[0].Replace("\n", string.Empty);
            _sendBuffer = Encoding.GetEncoding("UTF-8").GetBytes(outgoing[0] + '\n');
            clientSock.BeginSend(_sendBuffer, 0, _sendBuffer.Length, SocketFlags.None, new AsyncCallback(OnSent), null);
            outgoing.RemoveAt(0);
        }
	}

    public void AddNetworkMessage(string msg) {
        outgoing.Add(msg);
    }
}

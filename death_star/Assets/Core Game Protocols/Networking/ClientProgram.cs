using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class ClientProgram : MonoBehaviour {

    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    // Use this for initialization
    void Start () {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSock.Connect("178.128.95.63", 5000);
        clientSock.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(OnRecieve),null);

    }

    void OnRecieve(IAsyncResult asyncResult) {
        int recieved = clientSock.EndReceive(asyncResult);

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        Debug.Log(Encoding.Default.GetString(recData, 0, recData.Length));

        //Start receiving again
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

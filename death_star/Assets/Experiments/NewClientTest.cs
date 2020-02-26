using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class NewClientTest : MonoBehaviour {



    private Socket clientSock;
    private byte[] _recieveBuffer = new byte[8142];
    private byte[] _sendBuffer = new byte[8142];

    void Start() {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSock.Connect("178.128.95.63", 5000);

        byte[] intArr = BitConverter.GetBytes(20);


        bufferBuilder();
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

    void bufferBuilder() {
        byte[] test = Encoding.UTF8.GetBytes("What the fuck lmao hahahahaha i love this shit help me out.");
        int totalArrayLength = test.Length + 4;
        byte[] test1 = BitConverter.GetBytes(totalArrayLength);

        byte[] builtBuffer = new byte[totalArrayLength];
        Buffer.BlockCopy(test1, 0, builtBuffer, 0, 4);
        Buffer.BlockCopy(test, 0, builtBuffer, 4, test.Length);

        Debug.Log("Info len sent out: " + totalArrayLength);

        clientSock.BeginSend(builtBuffer, 0, totalArrayLength, SocketFlags.None, OnSent, null);
    }

    void OnSent(IAsyncResult asyncResult) {
        Debug.Log("Msg Sent 101");
    }

    void OnRecieve(IAsyncResult asyncResult) {
        Debug.Log("Recieved!");
        int recieved = clientSock.EndReceive(asyncResult);

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 4, recData, 0, recieved -4);

        Debug.Log("Bytes recieved " + Encoding.Default.GetString(recData));
        //Process data here the way you want , all your bytes will be stored in recData
        //Debug.Log(Encoding.Default.GetString(recData, 0, recData.Length));

        //Start receiving again

       //(Encoding.Default.GetString(recData, 0, recData.Length));
        clientSock.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnRecieve), null);
    }

   
}

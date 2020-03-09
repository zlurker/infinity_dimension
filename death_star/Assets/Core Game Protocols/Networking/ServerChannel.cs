using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerChannel : NetworkMessageEncoder {

    public override void MessageRecievedCallback() {
        ClientProgram.clientId = targetId;
        Debug.Log(targetId);
    }
}

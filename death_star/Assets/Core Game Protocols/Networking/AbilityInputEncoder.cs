using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityInputEncoder:NetworkMessageEncoder {

    public AbilityInputEncoder(int id):base(id) {

    }

   public void SendInputSignal(int abilityId) {
        bytesToSend = BitConverter.GetBytes(abilityId);
        SendEncodedMessages();
    }

    public override void RecieveEncodedMessages(byte[] msg) {
        base.RecieveEncodedMessages(msg);
    }

}

﻿using System.Collections;
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
        // Always ignore first 4 bytes.
        int abilityId = BitConverter.ToInt32(msg, 4);
        Debug.Log("AbilityId" + abilityId);

        TravelThread newAbilityThread = new TravelThread();

        // Adds created ability thread into networkobject list.
        NetworkObjectTracker.inst.AddNetworkObject(newAbilityThread);
        AbilitiesManager.aData[abilityId].CreateAbility(newAbilityThread);
    }

}

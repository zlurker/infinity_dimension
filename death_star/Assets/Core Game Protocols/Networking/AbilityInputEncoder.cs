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

    public override void MessageRecievedCallback() {
        int abilityId = BitConverter.ToInt32(bytesRecieved, 0);
        Debug.Log("AbilityId" + abilityId);

        AbilityCentralThreadPool newAbilityThread = new AbilityCentralThreadPool();

        // Adds created ability thread into networkobject list.
        NetworkObjectTracker.inst.AddNetworkObject(newAbilityThread);
        AbilitiesManager.aData[abilityId].CreateAbility(newAbilityThread);
    }
}

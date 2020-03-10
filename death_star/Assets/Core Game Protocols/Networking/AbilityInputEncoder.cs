using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityInputEncoder:NetworkMessageEncoder {

   public void SendInputSignal(int id) {
        SetBytesToSend(BitConverter.GetBytes(id));
    }

    public override void MessageRecievedCallback() {
        int aId = BitConverter.ToInt32(bytesRecieved, 0);
        Debug.Log("PlayerId" + targetId);
        Debug.Log("AbilityId" + aId);

        AbilityCentralThreadPool newAbilityThread = new AbilityCentralThreadPool(targetId);

        // Adds created ability thread into networkobject list.
        NetworkObjectTracker.inst.AddNetworkObject(newAbilityThread);
        AbilitiesManager.aData[targetId].abilties[aId].CreateAbility(newAbilityThread);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityInputEncoder : NetworkMessageEncoder {

    List<AbilityCentralThreadPool> playerGeneratedAbilities;

    public override void CalibrateEncoder(int id) {
        playerGeneratedAbilities = new List<AbilityCentralThreadPool>();
        base.CalibrateEncoder(id);
    }

    public void SendInputSignal(AbilityCentralThreadPool pool,int id, AbilityNodeNetworkData[] dataManifest) {

        playerGeneratedAbilities.Add(pool);

        byte[] idData = BitConverter.GetBytes(id);
        byte[] manifest = UpdateAbilityDataEncoder.PrepareVariableManifest(dataManifest);

        bytesToSend = new byte[idData.Length + manifest.Length];

        Buffer.BlockCopy(idData, 0, bytesToSend, 0, 4);
        Buffer.BlockCopy(manifest, 0, bytesToSend, 4, manifest.Length);

        SendEncodedMessages();
    }

    public override void MessageRecievedCallback() {
        if(targetId != ClientProgram.clientId) {
            int aId = BitConverter.ToInt32(bytesRecieved, 0);

            AbilityCentralThreadPool newAbilityThread = new AbilityCentralThreadPool(targetId);

            // Adds created ability thread into networkobject list.
            NetworkObjectTracker.inst.AddNetworkObject(newAbilityThread);
            AbilitiesManager.aData[targetId].abilties[aId].CreateAbility(newAbilityThread);

            UpdateAbilityDataEncoder.ParseManifest(newAbilityThread,bytesRecieved,4);
            //Debug.Log("(Input)Time end" + Time.realtimeSinceStartup);
        } else {
            NetworkObjectTracker.inst.AddNetworkObject(playerGeneratedAbilities[0]);
            playerGeneratedAbilities[0].RenameAllNodes();
            playerGeneratedAbilities.RemoveAt(0);
        }
    }
}

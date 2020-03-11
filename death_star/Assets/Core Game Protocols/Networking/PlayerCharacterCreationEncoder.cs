using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCreationEncoder : NetworkMessageEncoder {

    public override void MessageRecievedCallback() {

        /*// Adds created ability thread into networkobject list.
        NetworkObjectTracker.inst.AddNetworkObject(newAbilityThread);

        SpawnerOutput playerCharacter = LoadedData.GetSingleton<Spawner>().CreateScriptedObject(typeof(HealthSpawn));
        AbilitiesManager.aData[targetId].abilties[aId].CreateAbility(newAbilityThread);
        Debug.Log("(Input)Time end" + Time.realtimeSinceStartup);*/
    }
}

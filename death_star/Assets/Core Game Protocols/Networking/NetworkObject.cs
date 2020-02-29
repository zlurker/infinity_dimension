using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NetworkObject {
    void NetworkObjectCreationCallback(int networkObjId,int instId);
    int ReturnNetworkObjectId();
    int ReturnInstId();
}

public enum NetworkEncoderTypes {
    ABILITY_INPUT, UPDATE_ABILITY_DATA
}

public class NetworkObjectTracker:IGameplayStatic {

    public static NetworkObjectTracker inst;
    private EnhancedList<NetworkObject> networkObjects;
    private AutoPopulationList<int> instanceId;

    public void RunOnCreated() {
        networkObjects = new EnhancedList<NetworkObject>();
        instanceId = new AutoPopulationList<int>();
        inst = this;

        NetworkMessageEncoder.encoders = new NetworkMessageEncoder[] {
            new AbilityInputEncoder(),
            new UpdateAbilityDataEncoder()
        };

        for (int i=0; i < NetworkMessageEncoder.encoders.Length; i++) 
            NetworkMessageEncoder.encoders[i].SetEncoderId(i);        
    }

    public NetworkObjectTracker() {
        
    }

    public void AddNetworkObject(NetworkObject nO) {
        int networkObj = networkObjects.Add(nO);
        int currInstCount = instanceId.GetElementAt(networkObj);
        nO.NetworkObjectCreationCallback(networkObj,currInstCount);
    }

    public void DeleteNetworkObject(int objId) {
        networkObjects.Remove(objId);
        int currInstCount = instanceId.GetElementAt(objId);
        instanceId.ModifyElementAt(objId, currInstCount + 1);
    }

    public bool CheckIfInstIdMatches(int target,int given) {
        return given == instanceId.GetElementAt(target);
    }
}
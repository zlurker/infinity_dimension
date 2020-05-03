/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface NetworkObject {
    void NetworkObjectCreationCallback(int networkObjId,int instId);
    int ReturnNetworkObjectId();
    int ReturnInstId();
}

public class NetworkObjectTracker {

    public static NetworkObjectTracker inst;
    private EnhancedList<NetworkObject> networkObjects;
    private AutoPopulationList<int> instanceId;

    public NetworkObjectTracker() {
        networkObjects = new EnhancedList<NetworkObject>();
        instanceId = new AutoPopulationList<int>();
    }

    public NetworkObject ReturnNetworkObject(int id) {
        return networkObjects.l[id];
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
}*/
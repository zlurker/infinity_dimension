using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NetworkObject {
    void NetworkObjectCreationCallback(int networkObjId);
    int ReturnNetworkObjectId();
}

public class NetworkObjectTracker {

    public static NetworkObjectTracker inst;
    private EnhancedList<NetworkObject> networkObjects;
    private AutoPopulationList<int> instanceId;

    public NetworkObjectTracker() {
        networkObjects = new EnhancedList<NetworkObject>();
        instanceId = new AutoPopulationList<int>();
        inst = this;
    }

    public void AddNetworkObject(NetworkObject nO) {
        int networkObj = networkObjects.Add(nO);
        nO.NetworkObjectCreationCallback(networkObj);
    }

    public void DeleteNetworkObject(int objId) {
        networkObjects.Remove(objId);
        int currInstCount = instanceId.GetElementAt(objId);
        instanceId.ModifyElementAt(objId, currInstCount + 1);
    }
}
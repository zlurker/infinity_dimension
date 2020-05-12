using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeModifierLooper : NodeModifierBase, INodeNetworkPoint {

    protected int currLoop;
    protected Dictionary<int, List<AbilityNodeNetworkData>> pendingData = new Dictionary<int, List<AbilityNodeNetworkData>>();

    public void ModifyDataPacket(AbilityNodeNetworkData dataPacket) {
        dataPacket.additionalData = BitConverter.GetBytes(currLoop);
    }

    public void ProcessDataPacket<T>(AbilityNodeNetworkData<T> dataPacket) {
        int givenLoop = BitConverter.ToInt32(dataPacket.additionalData, 0);

        //Debug.LogFormat("{0},{1}", givenLoop, currLoop);

        // If its the same as curr loop, then apply value and not save the data anymore.
        if(givenLoop == currLoop) {
            GetCentralInst().UpdateVariableValue<T>(dataPacket.nodeId, dataPacket.varId, dataPacket.value,false);

//Debug.LogFormat("TNode {0}, Value {1}",GetCentralInst().GetNode(dataPacket.nodeId), dataPacket.value);

            int nTID = GetCentralInst().GetNode(dataPacket.nodeId).GetNodeThreadId();
            if(nTID > -1)
                GetCentralInst().UpdateVariableData<T>(nTID, dataPacket.varId);
            //GetCentralInst().UpdateVariableData<T>(nTID, dataPacket.varId);
            return;
        }

        if(!pendingData.ContainsKey(givenLoop))
            pendingData.Add(givenLoop, new List<AbilityNodeNetworkData>());

        // To be added for later use.
        pendingData[givenLoop].Add(dataPacket);
    }

    public void ApplyPendingDataToVariable(int iterationId) {
        if(pendingData.ContainsKey(iterationId)) {
            for(int i = 0; i < pendingData[iterationId].Count; i++) 
                pendingData[iterationId][i].ApplyDataToTargetVariable(GetCentralInst());

            pendingData.Remove(iterationId);
        }
    }
}

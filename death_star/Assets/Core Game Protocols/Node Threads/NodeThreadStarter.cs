using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeThreadStarter : AbilityTreeNode, INodeNetworkPoint {
    
    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<int>("placeholder", 0), VariableTypes.SIGNAL_ONLY));
    }

    public void ModifyDataPacket(AbilityNodeNetworkData dataPacket) {
    }

    public void ProcessDataPacket<T>(AbilityNodeNetworkData<T> dataPacket) {

        if(dataPacket.nodeId > -1) {
            int nTID = GetCentralInst().GetNode(dataPacket.nodeId).GetNodeThreadId();

            if(nTID > -1) {
                //Debug.Log("Input integrated.");
                GetCentralInst().UpdateVariableValue<T>(dataPacket.nodeId, dataPacket.varId, dataPacket.value);
                GetCentralInst().UpdateVariableData<T>(nTID, dataPacket.varId);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeThreadStarter : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        //base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<int>("", 0), VariableTypes.SIGNAL_ONLY));
    }

    public override void NodeCallback() {
    }

    /*public void ModifyDataPacket(AbilityNodeNetworkData dataPacket) {
    }

    public void ProcessDataPacket<T>(AbilityNodeNetworkData<T> dataPacket) {

        GetCentralInst().UpdateVariableValue<T>(dataPacket.nodeId, dataPacket.varId, dataPacket.value);
        GetCentralInst().UpdateVariableData<T>(dataPacket.nodeId, dataPacket.varId);
    }*/
}


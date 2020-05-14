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

        GetCentralInst().UpdateVariableValue<T>(dataPacket.nodeId, dataPacket.varId, dataPacket.value);
        GetCentralInst().UpdateVariableData<T>(dataPacket.nodeId, dataPacket.varId);
    }
}


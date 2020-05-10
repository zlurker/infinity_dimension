using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeThreadStarter : AbilityTreeNode, INodeNetworkPoint {
    
    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<int>("placeholder", 0), VariableTypes.SIGNAL_ONLY));
    }

    public byte[] ProcessNetworkData(AbilityNodeNetworkData dataPacket) {
    }
}

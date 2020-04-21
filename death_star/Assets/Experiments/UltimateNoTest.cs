using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateNoTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        Debug.Log(GetNodeVariable<float>("Num"));
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(
            new LoadedRuntimeParameters[] { new LoadedRuntimeParameters(new RuntimeParameters<float>("Num", 0), VariableTypes.AUTO_MANAGED)
        });      
    }
}

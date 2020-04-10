using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        GetCentralInst().NodeVariableCallback<float>(threadId, "Number",GetNodeVariable<float>("Number"));
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Number", 0), VariableTypes.AUTO_MANAGED)
        });
    }
}

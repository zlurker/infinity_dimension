using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateNoTest : AbilityTreeNode {

    private void Update() {
        Debug.Log(name + " " + GetNodeVariable<float>("Num"));
    }

    public override void NodeCallback() {
        base.NodeCallback();

        //Debug.Log(name + " " + GetNodeVariable<float>("Num"));
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(
            new LoadedRuntimeParameters[] { new LoadedRuntimeParameters(new RuntimeParameters<float>("Num", 0), VariableTypes.AUTO_MANAGED)
        });      
    }
}

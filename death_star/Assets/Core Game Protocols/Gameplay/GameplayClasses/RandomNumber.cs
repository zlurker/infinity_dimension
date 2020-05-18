using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumber : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Min Value",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Max Value",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Output Value",0),VariableTypes.HOST_ACTIVATED)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        float output = Random.Range(GetNodeVariable<float>("Min Value"), GetNodeVariable<float>("Max Value"));
        SetVariable<float>("Output Value", output);
    }
}

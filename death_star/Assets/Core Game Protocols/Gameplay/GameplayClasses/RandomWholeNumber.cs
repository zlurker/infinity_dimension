using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWholeNumber : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Min Value",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Max Value",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Output Value",0),VariableTypes.NETWORK)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        if(IsHost()) {
            int output = Random.Range(GetNodeVariable<int>("Min Value"), GetNodeVariable<int>("Max Value"));
            SetVariable<int>("Output Value", output);
        }
    }
}

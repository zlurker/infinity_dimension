using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<float>("Number", 0), VariableTypes.AUTO_MANAGED));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeNumber : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<int>("Number", 0), VariableTypes.AUTO_MANAGED));
    }
}

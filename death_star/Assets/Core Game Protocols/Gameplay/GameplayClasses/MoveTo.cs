using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        if(CheckIfVarRegionBlocked(0, 1)) {
            GetNodeVariable<AbilityTreeNode>("Target").transform.root.position = GetNodeVariable<Vector3>("Coordinates");
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
              new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Coordinates",new Vector3()),VariableTypes.AUTO_MANAGED),
              new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null),VariableTypes.AUTO_MANAGED)
        });
    }
}

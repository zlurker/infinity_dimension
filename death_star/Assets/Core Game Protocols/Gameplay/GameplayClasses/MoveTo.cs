using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        if(CheckIfVarRegionBlocked(0, 1)) 
            GetNodeVariable<AbilityTreeNode>("Target").transform.root.position = new Vector3(GetNodeVariable<Vector3>("Coordinates").x, GetNodeVariable<Vector3>("Coordinates").y, 0);       
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
              
              new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Coordinates",new Vector3()),VariableTypes.AUTO_MANAGED),
              new LoadedRuntimeParameters(new RuntimeParameters<bool>("Move all objects in blueprint", true),VariableTypes.AUTO_MANAGED),
              new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null),VariableTypes.AUTO_MANAGED)
        });
    }
}

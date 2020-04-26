using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : AbilityTreeNode {

    protected bool overrode;

    public override void NodeCallback() {
        base.NodeCallback();

        if(!overrode)
            if(CheckIfVarRegionBlocked("Coordinates", "Target"))
                GetTargetTransform().position = new Vector3(GetNodeVariable<Vector3>("Coordinates").x, GetNodeVariable<Vector3>("Coordinates").y, 0);
    }

    protected Transform GetTargetTransform() {
        if(GetNodeVariable<bool>("Move all objects in blueprint"))
            return GetNodeVariable<AbilityTreeNode>("Target").transform.root;
        else
            return GetNodeVariable<AbilityTreeNode>("Target").transform;
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

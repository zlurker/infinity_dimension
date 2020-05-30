using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectCoordinates : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(CheckIfVarRegionBlocked("Target Object", "Get Blueprint Position")) {

            Vector3 targetCoords;
            AbilityTreeNode target = GetNodeVariable<AbilityTreeNode>("Target Object");

            if(target == null)
                target = this;

            if(GetNodeVariable<bool>("Get Blueprint Position"))
                targetCoords = target.transform.position;
            else
                targetCoords = target.transform.parent.position;

            Debug.Log(targetCoords);
            SetVariable<Vector3>("Coordinates", targetCoords);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target Object",null),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Get Blueprint Position", true), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Coordinates", new Vector3()))
        });
    }
}

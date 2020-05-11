using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCoordinates : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(CheckIfVarRegionBlocked("Target")) {
            //Debug.Log("Passing coords");
            SetVariable<Vector3>("Target's Coordinates", GetNodeVariable<AbilityTreeNode>("Target").transform.root.position);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target", null),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Target's Coordinates", new Vector3()))
        });
    }
}

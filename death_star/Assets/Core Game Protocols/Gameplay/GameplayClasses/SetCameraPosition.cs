using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPosition : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(IsClientPlayerUpdate()) {
            
            Vector3 coords = GetNodeVariable<Vector3>("Coordinates");
            coords.z = -10;
            LoadedData.currSceneCamera.transform.position = coords;
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Coordinates", new Vector3()),VariableTypes.AUTO_MANAGED));
    }
}

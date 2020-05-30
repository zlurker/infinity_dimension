using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        bool allDataRecv = CheckIfVarRegionBlocked("X", "Y");

        if(allDataRecv) {
            Debug.LogFormat("Coordinates recieved: {0}, {1}",GetNodeVariable<float>("X"), GetNodeVariable<float>("Y")); 
            SetVariable<Vector3>("Output Coordinates", new Vector3(GetNodeVariable<float>("X"), GetNodeVariable<float>("Y")));
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("X",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Y",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Output Coordinates",new Vector3()))
        });
    }
}

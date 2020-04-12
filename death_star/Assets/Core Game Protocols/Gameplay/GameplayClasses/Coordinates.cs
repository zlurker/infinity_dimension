using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        bool allDataRecv = CheckIfVarRegionBlocked(0, 1);

        if(allDataRecv) {
            float[] coords = new float[] { GetNodeVariable<float>("X"), GetNodeVariable<float>("Y") };
            SetVariable("Output Coordinates", coords);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("X",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Y",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Output Coordinates",null))
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : AbilityTreeNode {

    public const int X = 0;
    public const int Y = 1;
    public const int OUTPUT_COORDINATES = 2;

    public override void NodeCallback(int threadId) {
        bool allDataRecv = CheckIfVarRegionBlocked(0, 1 );

        if(allDataRecv) {
            float[] coords = new float[] { GetNodeVariable<float>(X), GetNodeVariable<float>(Y) };
            GetCentralInst().NodeVariableCallback(GetNodeThreadId(), OUTPUT_COORDINATES, coords);
        }
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("X",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Y",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Output Coordinates",null))
        };
    }
}

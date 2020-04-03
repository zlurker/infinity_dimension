using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateNoTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        Debug.Log(GetNodeVariable<float>(0));
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Num",0))
        };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        Debug.Log("Number Test: " + GetNodeVariable<float>(0));
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Number",0))
        };
    }
}

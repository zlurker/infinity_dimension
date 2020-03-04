using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorLeftTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        GetCentralInst().NodeVariableCallback<float[]>(GetNodeThreadId(), 0, GetNodeVariable<float[]>(0));
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float[]>("Test", new float[]{1,0})
        };
    }
}

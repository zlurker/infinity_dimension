﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberTest : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        GetCentralInst().NodeVariableCallback<float>(threadId, 0,GetNodeVariable<float>(0));
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Number",0))
        };
    }
}
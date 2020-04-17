﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
              new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Coordinates",null),VariableTypes.AUTO_MANAGED),
              new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null),VariableTypes.AUTO_MANAGED)
        });
    }
}

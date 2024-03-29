﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberTest2 : AbilityTreeNode {


    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Number", 0), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Number2", 0), VariableTypes.AUTO_MANAGED)
        });
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        bool allDataRecv = CheckIfVarRegionBlocked("X", "Y");

        if(allDataRecv) 
            SetVariable<Vector3>("Output Coordinates", new Vector3(GetNodeVariable<float>("X"), GetNodeVariable<float>("Y")));        
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

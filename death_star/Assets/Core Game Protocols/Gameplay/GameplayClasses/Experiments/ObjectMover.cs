﻿/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        AbilityCentralThreadPool central = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        Vector3 movement = new Vector3(central.ReturnRuntimeParameter<float>(GetNodeId(), 1).v, central.ReturnRuntimeParameter<float>(GetNodeId(), 2).v);
        central.ReturnRuntimeParameter<GameObject>(GetNodeId(), 0).v.transform.position += movement;
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);
        
        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<GameObject>("Move Target", null),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("X", 0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Y", 0),VariableTypes.AUTO_MANAGED)       
        });
    }

}*/

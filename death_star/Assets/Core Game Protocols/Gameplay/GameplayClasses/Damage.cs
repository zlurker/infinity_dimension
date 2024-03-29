﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(CheckIfVarRegionBlocked("Damage", "Target")) {
            AbilityTreeNode target = GetNodeVariable<AbilityTreeNode>("Target");

            //Debug.Log(coords[0]);
            //AbilityCentralThreadPool central = NetworkObjectTracker.inst.ReturnNetworkObject(coords[0]) as AbilityCentralThreadPool;
            //HealthSpawn inst = globalList.l[central.GetAbilityNodeId()].abiNodes[coords[1]] as HealthSpawn;

            HealthSpawn inst = target as HealthSpawn;

            if(inst != null) {
                float currHp = target.GetNodeVariable<float>("Health");
                inst.SetVariable<float>("Health", currHp - GetNodeVariable<float>("Damage"));
            }
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Damage",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null),VariableTypes.AUTO_MANAGED)
        });
    }
}

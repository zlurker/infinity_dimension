﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : AbilityTreeNode {

    public const int DAMAGE = 0;
    public const int TARGET = 1;

    public override void NodeCallback(int threadId) {

        if(CheckIfVarRegionBlocked(0, 1)) {
            int[] coords = GetNodeVariable<int[]>(TARGET);

            AbilityCentralThreadPool central = NetworkObjectTracker.inst.ReturnNetworkObject(coords[0]) as AbilityCentralThreadPool;
            HealthSpawn inst = globalList.l[central.GetAbilityNodeId()].abiNodes[coords[1]] as HealthSpawn;

            if(inst != null) {
                RuntimeParameters<float> hpRp = inst.GetCentralInst().ReturnRuntimeParameter<float>(inst.GetNodeId(), HealthSpawn.HEALTH);
                hpRp.v -= GetNodeVariable<float>(DAMAGE);                
            }
        }
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Damage",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Target",null))
        };
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : AbilityTreeNode {

    public override void NodeCallback(int threadId) {

        if(CheckIfVarRegionBlocked(0, 1)) {
            int[] coords = GetNodeVariable<int[]>("Target");

            Debug.Log(coords[0]);
            AbilityCentralThreadPool central = NetworkObjectTracker.inst.ReturnNetworkObject(coords[0]) as AbilityCentralThreadPool;
            HealthSpawn inst = globalList.l[central.GetAbilityNodeId()].abiNodes[coords[1]] as HealthSpawn;
            
            if(inst != null) {
                RuntimeParameters<float> hpRp = inst.GetCentralInst().ReturnRuntimeParameter<float>(inst.GetNodeId(), "Health");
                hpRp.v -= GetNodeVariable<float>("Damage");
            }
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Damage",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Target",null),VariableTypes.AUTO_MANAGED)
        });
    }
}

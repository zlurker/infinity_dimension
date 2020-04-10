using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawn : SpawnerBase {

    void Update () {
        if(GetNodeVariable<float>("Health") < 0)
            transform.root.gameObject.SetActive(false);
	}

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {

        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Health",100),VariableTypes.AUTO_MANAGED)
        });
    }   
}

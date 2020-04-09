using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeSpawn : SpawnerBase, IOnSpawn {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);      
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters[]> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Spawn Lifetime",3),VariableTypes.AUTO_MANAGED)          
        });
    }
}

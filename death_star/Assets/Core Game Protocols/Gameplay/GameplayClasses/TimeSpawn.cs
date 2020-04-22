using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeSpawn : SpawnerBase, IOnSpawn {

    public override void NodeCallback() {
        base.NodeCallback();      
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Spawn Lifetime",3),VariableTypes.AUTO_MANAGED)          
        });
    }
}

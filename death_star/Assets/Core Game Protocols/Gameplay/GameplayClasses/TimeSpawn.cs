using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TimeSpawn : AbilityTreeNode {

    public const int SPAWN_LIFETIME =0;
    public const int TASKS = 1;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        GetCentralInst().NodeVariableCallback<object>(threadId, TASKS, null,VariableTypes.SIGNAL_VAR);        
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Spawn Lifetime",3),
            new RuntimeParameters<object>("Tasks",null)
        };
    }
}

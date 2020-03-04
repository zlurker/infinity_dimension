using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D))]
public class HealthSpawn : AbilityTreeNode {

    public const int HEALTH = 0;
    public const int TASKS = 1;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        inst.NodeVariableCallback<object>(threadId, TASKS, null);
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Health",100),
            new RuntimeParameters<object>("Tasks",null)
        };
    }
}

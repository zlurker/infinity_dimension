using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class TimeSpawn : AbilityTreeNode {

    public const int SPAWN_LIFETIME =0;
    public const int TASKS = 1;
    public const int ON_COLLIDE = 2;
    public const int SPRITE_FILE_PATH = 3;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(threadId, TASKS, this,VariableTypes.DEFAULT);        
    }

    public void OnCollisionStay2D(Collision2D collision) {
        
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Spawn Lifetime",3),
            new RuntimeParameters<AbilityTreeNode>("Spawn",null),
            new RuntimeParameters<object>("On Collide", null),
            new RuntimeParameters<string>("Sprite File Path", "")
        };
    }
}

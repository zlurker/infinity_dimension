using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D))]
public class HealthSpawn : AbilityTreeNode {

    public const int HEALTH = 0;
    public const int TASKS = 1;
    public const int ON_COLLIDE = 2;
    public const int SPRITE_FILE_PATH = 3;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {       
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(threadId, TASKS, this,VariableTypes.SIGNAL_VAR);
    }

    public void OnCollisionStay2D(Collision2D collision) {
        //collision.gameObject.name
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Health",100),
            new RuntimeParameters<AbilityTreeNode>("Spawn",null),
            new RuntimeParameters<object>("On Collide", null),
            new RuntimeParameters<string>("Sprite File Path", "")
        };
    }
}

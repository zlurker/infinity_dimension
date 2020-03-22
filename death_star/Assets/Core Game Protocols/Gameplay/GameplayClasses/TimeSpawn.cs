using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D),typeof(SpriteRenderer))]
public class TimeSpawn : AbilityTreeNode, IOnSpawn {

    public const int SPAWN_LIFETIME =0;
    public const int TASKS = 1;
    public const int ON_COLLIDE = 2;
    public const int SPRITE_FILE_PATH = 3;

    SpriteRenderer sR;
    Rigidbody2D rB;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        sR.sprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>(SPRITE_FILE_PATH)];
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(threadId, TASKS, this);        
    }

    public void OnCollisionStay2D(Collision2D collision) {
        
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Spawn Lifetime",3)),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Spawn",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<object>("On Collide", null)),
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG"))
        };
    }

    public void OnSpawn() {
        if(sR == null)
            sR = GetComponent<SpriteRenderer>();

        if(rB == null) {
            rB = GetComponent<Rigidbody2D>();
            rB.simulated = false;
        }
    }
}

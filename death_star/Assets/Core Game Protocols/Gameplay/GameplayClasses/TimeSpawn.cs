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
        Debug.Log(GetNodeVariable<string>(SPRITE_FILE_PATH));
        sR.sprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>(SPRITE_FILE_PATH)];
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(threadId, TASKS, this,VariableTypes.DEFAULT);        
    }

    public void OnCollisionStay2D(Collision2D collision) {
        
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Spawn Lifetime",3),
            new RuntimeParameters<AbilityTreeNode>("Spawn",null),
            new RuntimeParameters<object>("On Collide", null),
            new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG")
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

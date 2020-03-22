using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D),typeof(SpriteRenderer))]
public class HealthSpawn : AbilityTreeNode, IOnSpawn {

    public const int HEALTH = 0;
    public const int SPAWN = 1;
    public const int ON_COLLIDE = 2;
    public const int SPRITE_FILE_PATH = 3;

    SpriteRenderer sR;
    Rigidbody2D rB;

    void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        sR.sprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetCentralInst().ReturnRuntimeParameter<string>(GetNodeId(), SPRITE_FILE_PATH).v];
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(threadId, SPAWN, this,VariableTypes.DEFAULT);
    }

    public void OnCollisionStay2D(Collision2D collision) {

    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Health",100)),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Spawn",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<object>("On Collide", null),VariableTypes.HOST_ACTIVATED),
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

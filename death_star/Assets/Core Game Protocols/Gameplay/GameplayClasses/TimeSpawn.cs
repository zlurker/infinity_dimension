using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D),typeof(SpriteRenderer))]
public class TimeSpawn : AbilityTreeNode, IOnSpawn {

    public const int SPAWN_LIFETIME =0;
    public const int SPAWN = 1;
    public const int ON_COLLIDE = 2;
    public const int SPRITE_FILE_PATH = 3;

    SpriteRenderer sR;
    Rigidbody2D rB;

	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        sR.sprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>(SPRITE_FILE_PATH)];
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(GetNodeThreadId(), SPAWN, this);        
    }


    private void OnCollisionStay2D(Collision2D collision) {
        string[] objDetails = collision.gameObject.name.Split('/');
        int[] objLoc = new int[] { int.Parse(objDetails[0]), int.Parse(objDetails[1]) };
        GetCentralInst().NodeVariableCallback<int[]>(GetNodeThreadId(), ON_COLLIDE, objLoc);
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Spawn Lifetime",3),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Spawn",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("On Collide", null),VariableTypes.HOST_ACTIVATED),
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG"),VariableTypes.IMAGE_DEPENDENCY,VariableTypes.AUTO_MANAGED)
        };
    }

    public void OnSpawn() {
        if(sR == null)
            sR = GetComponent<SpriteRenderer>();

        if(rB == null) {
            rB = GetComponent<Rigidbody2D>();
            rB.gravityScale = 0;
            rB.mass = 0;
            rB.drag = 0;
            rB.angularDrag = 0;
            rB.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}

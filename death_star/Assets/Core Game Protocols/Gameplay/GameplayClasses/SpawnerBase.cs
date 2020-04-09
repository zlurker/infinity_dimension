using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class SpawnerBase : AbilityTreeNode, IOnSpawn {

    protected SpriteRenderer sR;
    protected Rigidbody2D rB;

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
        sR.sprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>(GetVariableId("Sprite File Path"))];
        GetCentralInst().NodeVariableCallback<AbilityTreeNode>(GetNodeThreadId(), GetVariableId("Spawn"), this);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        string[] objDetails = collision.gameObject.name.Split('/');
        int[] objLoc = new int[] { int.Parse(objDetails[0]), int.Parse(objDetails[1]) };
        GetCentralInst().NodeVariableCallback<int[]>(GetNodeThreadId(), GetVariableId("On Collide"), objLoc);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters[]> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Spawn",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("On Collide", null),VariableTypes.HOST_ACTIVATED),
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG"),VariableTypes.IMAGE_DEPENDENCY,VariableTypes.AUTO_MANAGED)
        });
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

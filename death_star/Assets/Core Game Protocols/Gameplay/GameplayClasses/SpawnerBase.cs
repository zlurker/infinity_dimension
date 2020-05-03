﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class SpawnerBase : SpriteSpawner, IOnSpawn {

    protected SpriteRenderer sR;
    protected Rigidbody2D rB;

    protected virtual void Update() {
        int[] objLoc = GetNodeVariable<int[]>("Internal Collide Handler");

        if(objLoc != null) {
            AbilityTreeNode inst = AbilitiesManager.aData[objLoc[0]].playerSpawnedCentrals.GetElementAt(objLoc[1]).GetNode(objLoc[2]);
            //AbilityCentralThreadPool central = NetworkObjectTracker.inst.ReturnNetworkObject(objLoc[0]) as AbilityCentralThreadPool;
            //AbilityTreeNode inst = GetCentralInst().GetNode(objLoc[1]);
            SetVariable<AbilityTreeNode>("On Collide", inst);
        }
    }

    public override void NodeCallback() {
        base.NodeCallback();
        sR.sprite = GetNodeVariable<Sprite>("Sprite");
           
    }

    private void OnCollisionStay2D(Collision2D collision) {

        string[] objDetails = collision.gameObject.name.Split('/');
        int[] objLoc = new int[] { int.Parse(objDetails[0]), int.Parse(objDetails[1]), int.Parse(objDetails[2]) };

        SetVariable("Internal Collide Handler", objLoc);       
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("On Collide", null)),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Internal Collide Handler", null),VariableTypes.HOST_ACTIVATED,VariableTypes.HIDDEN)
            //new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG"),VariableTypes.IMAGE_DEPENDENCY,VariableTypes.AUTO_MANAGED)
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

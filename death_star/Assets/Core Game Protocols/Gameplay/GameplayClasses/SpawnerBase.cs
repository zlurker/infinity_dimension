using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class SpawnerBase : SpriteSpawner, IOnVariableSet {


    protected Rigidbody2D rB;
    protected BoxCollider2D coll;

    public void OnVariableSet(int varId) {
        if(varId == GetVariableId("Internal Collide Handler")) {
            int[] objLoc = GetNodeVariable<int[]>("Internal Collide Handler");

            if(objLoc != null) {
                AbilityTreeNode inst = AbilitiesManager.aData[objLoc[0]].playerSpawnedCentrals.GetElementAt(objLoc[1]).GetNode(objLoc[2]);
                //AbilityCentralThreadPool central = NetworkObjectTracker.inst.ReturnNetworkObject(objLoc[0]) as AbilityCentralThreadPool;
                //AbilityTreeNode inst = GetCentralInst().GetNode(objLoc[1]);
                SetVariable<AbilityTreeNode>("On Collide", inst);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {

        if(IsHost() && GetNodeThreadId() > -1) {
            string[] objDetails = collision.gameObject.name.Split('/');
            int[] objLoc = new int[] { int.Parse(objDetails[0]), int.Parse(objDetails[1]), int.Parse(objDetails[2]) };

            if(GetNodeVariable<bool>("Don't Hit Self")) {
                int playerId = AbilitiesManager.aData[objLoc[0]].playerSpawnedCentrals.GetElementAt(objLoc[1]).GetPlayerId();

                if(playerId == GetCentralInst().GetPlayerId())
                    return;
            }

            SetVariable("Internal Collide Handler", objLoc);
        }
    }

    public override void NodeCallback() {
        base.NodeCallback();

        coll.enabled = GetNodeVariable<bool>("Enable Collision");
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("On Collide", null)),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Internal Collide Handler", null),VariableTypes.NETWORK,VariableTypes.HIDDEN),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Enable Collision",true),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Don't Hit Self",true),VariableTypes.AUTO_MANAGED)
            //new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite File Path", "Bullet.PNG"),VariableTypes.IMAGE_DEPENDENCY,VariableTypes.AUTO_MANAGED)
        });
    }

    public override void OnSpawn() {
        base.OnSpawn();

        if(rB == null) {
            rB = GetComponent<Rigidbody2D>();
            rB.gravityScale = 0;
            rB.mass = 0;
            rB.drag = 0;
            rB.angularDrag = 0;
            rB.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if(coll == null)
            coll = GetComponent<BoxCollider2D>();

    }

}

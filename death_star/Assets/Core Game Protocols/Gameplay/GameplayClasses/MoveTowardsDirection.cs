using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode,IOnSpawn {

    public const int DIRECTION_FROM_TARGET = 0;
    public const int SPEED = 1;
    public const int TARGET = 2;

    bool allDataRecv;
    Vector3 direction;

	void Update () {
        if(allDataRecv) {
            Debug.Log("Working");
            GetNodeVariable<AbilityTreeNode>(TARGET).transform.root.position += direction;
        }
	}

    public override void NodeCallback(int threadId) {
        allDataRecv = CheckIfVarRegionBlocked(new int[] { 0,1,2 });

        if(allDataRecv) {
            float[] vectorHolder = GetNodeVariable<float[]>(DIRECTION_FROM_TARGET);
            direction = new Vector3(vectorHolder[0],vectorHolder[1]).normalized * GetNodeVariable<float>(SPEED);
        }      
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float[]>("Direction From Target",null),
            new RuntimeParameters<float>("Speed",1),
            new RuntimeParameters<AbilityTreeNode>("Target",null)
        };
    }

    public void OnSpawn() {
        allDataRecv = false;
    }
}

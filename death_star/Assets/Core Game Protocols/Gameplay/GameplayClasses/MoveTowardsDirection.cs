using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode,IOnSpawn {

    public const int DIRECTION_FROM_TARGET = 0;
    public const int TARGET = 1;
    bool allDataRecv;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(allDataRecv) {

        }
	}

    public override void NodeCallback(int threadId) {
        allDataRecv = CheckIfVarRegionBlocked(new int[] { 0,1 });
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float[]>("Direction From Target",null),
            new RuntimeParameters<GameObject>("Target",null)
        };
    }

    public void OnSpawn() {
        allDataRecv = false;
    }
}

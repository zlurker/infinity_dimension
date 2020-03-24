using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode,IOnSpawn {

    public const int DIRECTION_FROM_TARGET = 0;
    public const int SPEED_PER_SECOND = 1;
    public const int TARGET = 2;

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

	void Update () {
        
        if(allDataRecv) {
            //Debug.Log(GetNodeVariable<AbilityTreeNode>(TARGET));
            GetNodeVariable<AbilityTreeNode>(TARGET).transform.root.position = dirChangeStart + ((Time.realtimeSinceStartup - timeDirChanged) * normDir);
        }
	}

    public override void NodeCallback(int threadId) {

        allDataRecv = CheckIfVarRegionBlocked(new int[] { 0,1,2 });

        if(allDataRecv) {
            float[] vectorHolder = GetNodeVariable<float[]>(DIRECTION_FROM_TARGET);
            normDir = new Vector3(vectorHolder[0],vectorHolder[1]).normalized;

            dirChangeStart = transform.position;
            timeDirChanged = Time.realtimeSinceStartup;
        }      
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Direction From Target",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Speed Per Second",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null))
        };
    }

    public void OnSpawn() {
        allDataRecv = false;
    }
}

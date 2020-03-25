using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode,IOnSpawn {

    public const int DIRECTION_FROM_TARGET = 0;
    public const int TOTAL_DISTANCE = 1;
    public const int DURATION = 2;
    public const int TARGET = 3;

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

	void Update () {
        
        if(allDataRecv) {

            if(GetNodeVariable<float>(DURATION) > Time.realtimeSinceStartup - timeDirChanged) {
                float timeRatio = (Time.realtimeSinceStartup - timeDirChanged)/GetNodeVariable<float>(DURATION);
                GetNodeVariable<AbilityTreeNode>(TARGET).transform.root.position = dirChangeStart + (normDir * (GetNodeVariable<float>(TOTAL_DISTANCE) * timeRatio));
            } else
                GetNodeVariable<AbilityTreeNode>(TARGET).transform.root.position = dirChangeStart + (normDir * GetNodeVariable<float>(TOTAL_DISTANCE));
        }
	}

    public override void NodeCallback(int threadId) {

        allDataRecv = CheckIfVarRegionBlocked(new int[] { 0,1,2,3 });

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
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Total Distance",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Duration",1)),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null))
        };
    }

    public void OnSpawn() {
        allDataRecv = false;
    }
}

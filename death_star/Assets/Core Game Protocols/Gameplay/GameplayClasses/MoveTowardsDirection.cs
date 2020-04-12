using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode,IOnSpawn {

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

	void Update () {
        
        if(allDataRecv) {

            if(GetNodeVariable<float>("Duration") > Time.realtimeSinceStartup - timeDirChanged) {
                float timeRatio = (Time.realtimeSinceStartup - timeDirChanged)/GetNodeVariable<float>("Duration");
                GetNodeVariable<AbilityTreeNode>("Target").transform.root.position = dirChangeStart + (normDir * (GetNodeVariable<float>("Total Distance") * timeRatio));
            } else
                GetNodeVariable<AbilityTreeNode>("Target").transform.root.position = dirChangeStart + (normDir * GetNodeVariable<float>("Total Distance"));
        }
	}

    public override void NodeCallback(int threadId) {

        allDataRecv = CheckIfVarRegionBlocked(0,1,2,3);

        //Debug.Log("Callback from " + threadId);

        if(allDataRecv) {
            float[] vectorHolder = GetNodeVariable<float[]>("Direction From Target");
            normDir = new Vector3(vectorHolder[0],vectorHolder[1]).normalized;

            dirChangeStart = transform.position;
            timeDirChanged = Time.realtimeSinceStartup;            
        }      
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Direction From Target",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Total Distance",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Duration",1)),
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Target",null))
        });
    }

    public void OnSpawn() {
        allDataRecv = false;
    }
}

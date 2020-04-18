using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : MoveTo {

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

    void Update() {

        if(allDataRecv) {
            if(GetNodeVariable<float>("Duration") > Time.realtimeSinceStartup - timeDirChanged) {
                float timeRatio = (Time.realtimeSinceStartup - timeDirChanged) / GetNodeVariable<float>("Duration");

                GetTargetTransform().position = dirChangeStart + (normDir * (GetNodeVariable<float>("Total Distance") * timeRatio));
            } else
                GetTargetTransform().position = dirChangeStart + (normDir * GetNodeVariable<float>("Total Distance"));

            GetTargetTransform().position = new Vector3(GetTargetTransform().position.x, GetTargetTransform().position.y, 0);
        }
    }

    public override void NodeCallback(int threadId) {

        allDataRecv = CheckIfVarRegionBlocked("Coordinates", "Target", "Total Distance", "Duration");

        if(allDataRecv) {
            normDir = GetNodeVariable<Vector3>("Coordinates").normalized;

            dirChangeStart = transform.position;
            timeDirChanged = Time.realtimeSinceStartup;
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Total Distance",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Duration",1))
        });
    }
}

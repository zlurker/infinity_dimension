using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : MoveTo {

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

    void Update() {

        if(allDataRecv)
            if(GetNodeVariable<float>("Duration") > Time.realtimeSinceStartup - timeDirChanged) {
                float timeRatio = (Time.realtimeSinceStartup - timeDirChanged) / GetNodeVariable<float>("Duration");

                GetTargetTransform().position = dirChangeStart + (normDir * (GetNodeVariable<float>("Total Distance") * timeRatio));
            } else
                GetTargetTransform().position = dirChangeStart + (normDir * GetNodeVariable<float>("Total Distance"));
    }

    public override void NodeCallback() {
        setTarget = false;
        base.NodeCallback();

        allDataRecv = CheckIfVarRegionBlocked("Coordinates", "Target", "Total Distance", "Duration");

        if(allDataRecv) {
            normDir = GetNodeVariable<Vector3>("Coordinates").normalized;
            normDir.z = 0;

            dirChangeStart = GetTargetTransform().position;
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

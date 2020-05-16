using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It will inherit from NML. However, it will not make use of Threadmaps.
public class Repeater : NodeModifierLooper, IOnSpawn {

    float startTime = -1;

	void Update () {
        if(currLoop < GetNodeVariable<int>("Total Repeatable Times"))
		if (startTime > -1) {
            float diff = Time.realtimeSinceStartup - startTime;
            int diffMultiplier = Mathf.FloorToInt(diff / GetNodeVariable<float>("Time Interval"));

            for(int i = 0; i < diffMultiplier; i++) {
                currLoop++;
                BeginRepeater();
            }

            if(diffMultiplier > 0)
                startTime = Time.realtimeSinceStartup;
        }
	}

    public override void NodeCallback() {
        destroyOverridenThreads = true;

        base.NodeCallback();

        if(startTime == -1) {
            startTime = Time.realtimeSinceStartup;
            BeginRepeater(); 
        }        
    }

    public void BeginRepeater() {

        ApplyPendingDataToVariable(currLoop);

        Debug.Log("Repeater running.");

        // Fires a phantom thread.
        NodeThread trdInst = new NodeThread();
        trdInst.SetNodeData(GetNodeId(), GetCentralInst().GetNodeBranchData(GetNodeId()));

        int threadToUse = GetCentralInst().AddNewThread(trdInst);

        SetVariable<float>(threadToUse,"Time Interval");       
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Time Interval", 1)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Total Repeatable Times", 10))
        });
    }

    public void OnSpawn() {
        startTime = -1;
    }
}

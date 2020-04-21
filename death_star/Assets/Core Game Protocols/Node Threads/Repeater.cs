using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : AbilityTreeNode, IOnSpawn {

    float startTime = -1;

	void Update () {
		if (startTime > -1) {
            float diff = Time.realtimeSinceStartup - startTime;
            int diffMultiplier = Mathf.FloorToInt(diff / GetNodeVariable<float>("Time Interval"));

            for(int i = 0; i < diffMultiplier; i++)
                BeginRepeater();

            if(diffMultiplier > 0)
                startTime = Time.realtimeSinceStartup;
        }
	}

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        if(startTime == -1) {
            startTime = Time.realtimeSinceStartup;
            BeginRepeater();
        }        
    }

    public void BeginRepeater() {
        NodeThread trdInst = new NodeThread(GetNodeId());
        trdInst.SetNodeData(GetNodeId(), GetCentralInst().GetNodeBranchData(GetNodeId()));

        int threadToUse = GetCentralInst().AddNewThread(trdInst);
        Debug.Log("Launching repeater...");
        Debug.Log(threadToUse);
        SetVariable("Time Interval", 0);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Time Interval", 1))
        });
    }

    public void OnSpawn() {
        startTime = -1;
    }
}

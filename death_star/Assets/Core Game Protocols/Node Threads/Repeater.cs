using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : AbilityTreeNode, IOnSpawn {

    public const int TIME_INTERVAL = 0;

    float startTime = -1;

	void Update () {
		if (startTime > -1) {
            float diff = Time.realtimeSinceStartup - startTime;
            int diffMultiplier = Mathf.FloorToInt(diff / GetNodeVariable<float>(TIME_INTERVAL));

            for(int i = 0; i < diffMultiplier; i++)
                BeginRepeater();

            if(diffMultiplier > 0)
                startTime = Time.realtimeSinceStartup;
        }
	}

    public override void NodeCallback(int threadId) {

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
        GetCentralInst().NodeVariableCallback<float>(threadToUse, TIME_INTERVAL, 0);       
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Time Interval",1))
        };
    }

    public void OnSpawn() {
        startTime = -1;
    }
}

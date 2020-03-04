using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : AbilityTreeNode, IOnSpawn {

    public const int TIME_INTERVAL = 0;

    float startTime = -1;

	void Update () {
		if (startTime > -1) {
            float diff = Time.realtimeSinceStartup - startTime;
            int diffMultiplier = Mathf.FloorToInt(diff / AbilityCentralThreadPool.globalCentralList.l[GetCentralId()].ReturnRuntimeParameter<float>(GetNodeId(),TIME_INTERVAL).v);

            for(int i = 0; i < diffMultiplier; i++)
                BeginRepeater();

            if(diffMultiplier > 0)
                startTime = Time.realtimeSinceStartup;
        }
	}

    public override void NodeCallback(int threadId) {
        Debug.Log(threadId);

        if(startTime == -1) {
            startTime = Time.realtimeSinceStartup;
            BeginRepeater();
        }

        
    }

    public void BeginRepeater() {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        NodeThread trdInst = new NodeThread(GetNodeId());
        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

        int threadToUse = inst.AddNewThread(trdInst);
        Debug.Log("Launching repeater...");
        Debug.Log(threadToUse);
        inst.NodeVariableCallback<float>(threadToUse, 0, 0);       
    }



    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Time Interval",1)
        };
    }

    public void OnSpawn() {
        startTime = -1;
    }
}

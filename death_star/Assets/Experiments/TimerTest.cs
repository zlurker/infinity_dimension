using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest : AbilityTreeNode {


    float initialTimer;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if (initialTimer + 6 < Time.time && initialTimer > 0) {
            Debug.Log("Current node thread:" + GetNodeThreadId());
            Debug.LogFormat("curr node {0}, nodeValue{1}", GetNodeId(), TravelThread.globalCentralList.l[GetCentralId()].ReturnVariable<string>(GetNodeId(), 0).v);
            TravelThread.globalCentralList.l[GetCentralId()].NodeVariableCallback<string>(GetNodeThreadId(), 0, "Mission Sucess!");
            Debug.Log("Current node thread:" + GetNodeThreadId());
            initialTimer = -1;
        }

	}

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Testing","12345")
        };
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        initialTimer = Time.time;
    }

}

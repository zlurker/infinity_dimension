﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnValue : AbilityTreeNode {

    
    Dictionary<int, int> threadMap = new Dictionary<int, int>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override RuntimeParameters[] GetRuntimeParameters() {
        // I need to think of a way to make it such that it can accept any type of variable.
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Extended Path", 0),
            new RuntimeParameters<int>("Return from Variable", 0)
        };
    }

    public override void NodeCallback(int threadId) {
        threadMap.Add(threadId, 0);

        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), threadId);
        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

        int threadToUse = inst.AddNewThread(trdInst);
        Debug.LogFormat("Thread id {0} has been created.", threadToUse);
        inst.NodeVariableCallback<int>(threadToUse, 0, 20);
    }

    public override void ThreadEndStartCallback(int threadId) {
        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        //Debug.LogFormat("Thread id {0} has finished looping.", threadId);

        if(nT is ChildThread) {

            int parentThread = (nT as ChildThread).GetOriginalThread();

            threadMap[parentThread] += 1;

            Debug.LogFormat("Thread id {0}, current node collection progress {1}/{2}", threadId, threadMap[parentThread], inst.GetSpecialisedNodeData(GetNodeId()));

            if(threadMap[parentThread] >= inst.GetSpecialisedNodeData(GetNodeId())) {
                // Return value of target.
                Variable storedAddress = inst.ReturnVariable(GetNodeId(), 1);
                //storedAddress.links[1][storedAddress.links[1].Length-1][0]
               // Debug.Log("Variable returned: " + inst.ReturnRuntimeParameter<int>(storedAddress.links[storedAddress.links[1].Length - 1][0], storedAddress.links[1][storedAddress.links[1].Length - 1][1]));
            }
        }
    }
}
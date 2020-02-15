using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSplitter : AbilityTreeNode {

    Hashtable threadSet;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Number of Loops", 1)
        };
    }

    public override void NodeCallback(int threadId) {

        if(threadSet == null)
            threadSet = new Hashtable();

        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];
        int threadToUse = threadId;
        int len = inst.ReturnVariable<int>(GetNodeId(), 0).v;
          
        NodeThread trdInst = new NodeThread(GetNodeId(), len);

        if(!threadSet.ContainsKey(threadId)) {            
            threadToUse = inst.AddNewThread(trdInst);
            threadSet.Add(threadToUse, 0);
        }else
            Debug.Log("Looping!!");

        TravelThread.globalCentralList.l[GetCentralId()].NodeVariableCallback<int>(threadToUse, 0, len);
    }

    public override void ThreadEndStartCallback(int threadId) {
        //Debug.Log(GetNodeThreadId());
        IncrementLoop(GetNodeThreadId());
    }

    public void IncrementLoop(int threadId) {
        NodeThread inst = TravelThread.globalCentralList.l[GetCentralId()].GetActiveThread(threadId);
        int jointThread = inst.GetJointThread();
        inst.IncrementCompletion();
        TravelThread.globalCentralList.l[GetCentralId()].SeeNodeThreadLoop(threadId);

        if(jointThread > -1)
            IncrementLoop(jointThread);
    }
}

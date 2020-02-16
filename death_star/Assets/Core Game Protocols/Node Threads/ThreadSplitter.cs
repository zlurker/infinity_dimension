using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSplitter : AbilityTreeNode {

    public class ChildThread : NodeThread {

        int originalThread;

        public ChildThread(int sPt,int l, int oT): base (sPt,l) {
            originalThread = oT;
        }
    }

	// Use this for initialization
	/*void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Number of Loops", 1)
        };
    }

    public override void NodeCallback(int threadId) {


        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];
        int threadToUse = threadId;
        int len = inst.ReturnVariable<int>(GetNodeId(), 0).v;
          
        if(!(inst.GetActiveThread(threadId) is ChildThread)) {
            ChildThread trdInst = new ChildThread(GetNodeId(), len,threadId);
            threadToUse = inst.AddNewThread(trdInst);
            //threadSet.Add(threadToUse, 0);
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

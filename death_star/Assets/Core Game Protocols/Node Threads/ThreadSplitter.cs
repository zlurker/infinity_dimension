using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSplitter : AbilityTreeNode {

    public class ChildThread : NodeThread {

        int originalThread;

        public ChildThread(int sPt, int l, int oT) : base(sPt, l) {
            originalThread = oT;
        }
    }

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
            ChildThread trdInst = new ChildThread(GetNodeId(), len, threadId);
            threadToUse = inst.AddNewThread(trdInst);
        } else {
            Debug.Log("Looping, allow node override/join.");
            inst.GetActiveThread(threadId).SetJoin(true);
            inst.GetActiveThread(threadId).SetOverride(true);
        }

        TravelThread.globalCentralList.l[GetCentralId()].NodeVariableCallback<int>(threadToUse, 0, len);
    }

    public override void ThreadEndStartCallback(int threadId) {
        IncrementLoop(GetNodeThreadId());
    }

    public override void OnLoopThreadBegin(int threadId) {
        NodeThread inst = TravelThread.globalCentralList.l[GetCentralId()].GetActiveThread(threadId);

        Debug.Log("Privileges removed, child thread detected.");
        inst.SetJoin(false);
        inst.SetOverride(false);
    }

    public void IncrementLoop(int threadId) {

        NodeThread inst = TravelThread.globalCentralList.l[GetCentralId()].GetActiveThread(threadId);
        int jointThread = inst.GetJointThread();
        inst.IncrementCompletion();

        /*Debug.Log("At loop increment.");

        if(inst is ChildThread) {
            Debug.Log("Priviledges removed, child thread detected.");
            inst.SetJoin(false);
            inst.SetOverride(false);
        }*/

        TravelThread.globalCentralList.l[GetCentralId()].SeeNodeThreadLoop(threadId);

        if(jointThread > -1)
            IncrementLoop(jointThread);
    }
}

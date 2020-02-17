using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSplitter : AbilityTreeNode {

    public class ChildThread : NodeThread {

        int originalThread;

        public ChildThread(int sPt, int oT) : base(sPt) {
            originalThread = oT;
        }

        public override NodeThread CreateNewThread() {
            generatedNodeThreads++;

            if(possiblePaths > generatedNodeThreads) 
                return new ChildThread(GetStartingPoint(), originalThread);
            
            return null;
        }

        public int GetOriginalThread() {
            return originalThread;
        }
    }

    Dictionary<int, int[]> threadMap = new Dictionary<int, int[]>();

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Number of Loops", 1)
        };
    }

    public override void NodeCallback(int threadId) {
        threadMap.Add(threadId, new int[2]);
        ProcessThreads(threadId);
    }

    public override void ThreadEndStartCallback(int threadId) {
        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        Debug.LogFormat("Thread id {0} has finished looping.", threadId);
        
        if(nT is ChildThread) {
            
            int parentThread = (nT as ChildThread).GetOriginalThread();

            threadMap[parentThread][1] += 1;

            Debug.LogFormat("Thread id {0}, current node collection progress {1}/{2}", threadId, threadMap[parentThread][1], inst.GetSpecialisedNodeData(GetNodeId()));

            if(threadMap[parentThread][1] >= inst.GetSpecialisedNodeData(GetNodeId())) {
                // Resets thread counter and adds one to loop counter.
                threadMap[parentThread][0] += 1;
                threadMap[parentThread][1] = 0;
                ProcessThreads(parentThread);
            }         
        }
    }

    public void ProcessThreads(int threadId) {

        TravelThread inst = TravelThread.globalCentralList.l[GetCentralId()];

        Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, threadMap[threadId][0], inst.ReturnVariable<int>(GetNodeId(), 0).v);

        if(threadMap[threadId][0] < inst.ReturnVariable<int>(GetNodeId(), 0).v) {
            Debug.LogFormat("Thread id {0} will reloop.", threadId);
            ChildThread trdInst = new ChildThread(GetNodeId(), threadId);
            trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

            int threadToUse = inst.AddNewThread(trdInst);
            Debug.LogFormat("Thread id {0} has been created.", threadToUse);
            inst.NodeVariableCallback<int>(threadToUse, 0, 20);
        } else {
            Debug.LogFormat("Thread id {0} will end.", threadId);
            inst.ThreadEndCallback(threadId);
        }
    }

    /*public override void OnLoopThreadBegin(int threadId) {
        NodeThread inst = TravelThread.globalCentralList.l[GetCentralId()].GetActiveThread(threadId);

        Debug.Log("Privileges removed, child thread detected.");
        inst.SetJoin(false);
        inst.SetOverride(false);
    }*/

    /*public void IncrementLoop(int threadId) {

        NodeThread inst = TravelThread.globalCentralList.l[GetCentralId()].GetActiveThread(threadId);
        int jointThread = inst.GetJointThread();
        inst.IncrementCompletion();

        TravelThread.globalCentralList.l[GetCentralId()].SeeNodeThreadLoop(threadId);

        if(jointThread > -1)
            IncrementLoop(jointThread);
    }*/
}

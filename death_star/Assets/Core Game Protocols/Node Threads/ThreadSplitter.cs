using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSplitter : AbilityTreeNode {

    public const int NUMBER_OF_LOOPS = 0;

    protected Dictionary<int, int[]> threadMap = new Dictionary<int, int[]>();

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Number of Loops", 1))
        };
    }

    public override void NodeCallback(int threadId) {
        Debug.LogFormat("Thread ID {0} has called threadsplitter.", threadId);
        threadMap.Add(threadId, new int[2]);
        ProcessThreads(threadId);
    }

    public override void ThreadEndStartCallback(int threadId) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        //Debug.LogFormat("Thread id {0} has finished looping. Returned to {1}", threadId, (nT as ChildThread).GetOriginalThread());
        
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

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, threadMap[threadId][0], inst.ReturnRuntimeParameter<int>(GetNodeId(), 0).v);
      
        if(threadMap[threadId][0] < GetNodeVariable<int>(NUMBER_OF_LOOPS) || GetNodeVariable<int>(NUMBER_OF_LOOPS) ==-1) {
            ChildThread trdInst = new ChildThread(GetNodeId(), threadId);
            trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

            int threadToUse = inst.AddNewThread(trdInst);
            Debug.LogFormat("Thread id {0} has been created.", threadToUse);
            inst.NodeVariableCallback<int>(threadToUse, NUMBER_OF_LOOPS, 0);
        } else {
            Debug.LogFormat("Thread id {0} will end.", threadId);
            inst.HandleThreadRemoval(threadId);
            threadMap.Remove(threadId);

            if(threadMap.Count == 0) {
                Debug.Log("Threadmap empty. Setting node thread id to -1.");
                SetNodeThreadId(-1);
            }
        }
    }
}

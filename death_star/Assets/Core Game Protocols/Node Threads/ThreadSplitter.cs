using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterData : ThreadMapDataBase {
    public int numberOfLoops;

    public SplitterData() {
        numberOfLoops = 0;
    }
}

public class ThreadSplitter : NodeModifierBase {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Number of Loops", 1),VariableTypes.SIGNAL_ONLY)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        Debug.LogFormat("Thread ID {0} has called threadsplitter.", GetNodeThreadId());
        threadMap.Add(GetNodeThreadId(), new SplitterData());
        ProcessThreads(GetNodeThreadId());
    }


    public override void ThreadZeroed(int parentThread, int lastChildThread) {
        (threadMap[parentThread] as SplitterData).numberOfLoops++;

        if(GetCentralInst().GetActiveThread(lastChildThread).GetJointThread() == -1)
            GetCentralInst().SetTimerEventID(-1);

        ProcessThreads(parentThread);
    }

    public void ProcessThreads(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        SplitterData sData = threadMap[threadId] as SplitterData;

        //Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, sData.numberOfLoops, inst.ReturnRuntimeParameter<int>(GetNodeId(), 0).v);

        if(sData.numberOfLoops < GetNodeVariable<int>("Number of Loops") || GetNodeVariable<int>("Number of Loops") == -1) {
            ChildThread trdInst = new ChildThread(GetNodeId(), threadId, this);
            trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

            int threadToUse = inst.AddNewThread(trdInst);
            Debug.LogFormat("Thread id {0} has been created.", threadToUse);
            inst.NodeVariableCallback<int>(threadToUse, GetVariableId("Number of Loops"));
        } else {
            inst.HandleThreadRemoval(threadId);
            threadMap.Remove(threadId);
        }
    }
}

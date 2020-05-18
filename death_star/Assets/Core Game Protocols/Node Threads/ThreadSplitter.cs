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

    /*int threadToProcess = -1;

    private void Update() {

        if(threadToProcess > -1) {
            ProcessThreads(threadToProcess);
            threadToProcess = -1;
        }
    }*/

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Number of Loops", 1),VariableTypes.SIGNAL_ONLY)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        threadMap.Add(latestThread, new SplitterData());
        ProcessThreads(latestThread);
    }


    public override void ThreadZeroed(int parentThread) {

        //Debug.Log("Loop completed.");
        (threadMap[parentThread] as SplitterData).numberOfLoops++;
        //currLoop++;
        ProcessThreads(parentThread);
        //threadToProcess = parentThread;
    }

    public void ProcessThreads(int threadId) {

        //ApplyPendingDataToVariable(currLoop);

        AbilityCentralThreadPool inst = GetCentralInst();
        SplitterData sData = threadMap[threadId] as SplitterData;

        //Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, sData.numberOfLoops, inst.ReturnRuntimeParameter<int>(GetNodeId(), 0).v);

        if(sData.numberOfLoops < GetNodeVariable<int>("Number of Loops") || GetNodeVariable<int>("Number of Loops") == -1) {
            NodeModifierBaseThread trdInst = new NodeModifierBaseThread(threadId, this);
            trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

            int threadToUse = inst.AddNewThread(trdInst);
            //Debug.LogFormat("Thread {0} is starting a new journey.", threadToUse);
            //Debug.LogFormat("Thread id {0} has been created.", threadToUse);
            SetVariable<int>(threadToUse, "Number of Loops");
        } else {
            //Debug.LogFormat("Thread {0} was rmed.", threadId);
            inst.HandleThreadRemoval(threadId);
            threadMap.Remove(threadId);
        }
    }
}

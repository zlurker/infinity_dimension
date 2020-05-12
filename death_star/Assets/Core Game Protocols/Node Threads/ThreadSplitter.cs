﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterData : ThreadMapDataBase {
    public int numberOfLoops;

    public SplitterData() {
        numberOfLoops = 0;
    }
}

public class ThreadSplitter : NodeModifierLooper {

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
        (threadMap[parentThread] as SplitterData).numberOfLoops++;
        currLoop++;
        GetCentralInst().SetTimerEventID(-1);
        ProcessThreads(parentThread);
    }

    public void ProcessThreads(int threadId) {

        ApplyPendingDataToVariable(currLoop);

        AbilityCentralThreadPool inst = GetCentralInst();
        SplitterData sData = threadMap[threadId] as SplitterData;

        //Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, sData.numberOfLoops, inst.ReturnRuntimeParameter<int>(GetNodeId(), 0).v);

        if(sData.numberOfLoops < GetNodeVariable<int>("Number of Loops") || GetNodeVariable<int>("Number of Loops") == -1) {
            NodeThread trdInst = new NodeThread(threadId, this);
            trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

            int threadToUse = inst.AddNewThread(trdInst);
            //Debug.LogFormat("Thread {0} is starting a new journey.", threadToUse);
            //Debug.LogFormat("Thread id {0} has been created.", threadToUse);
            SetVariable<int>(threadToUse, "Number of Loops");
        } else {
            Debug.LogFormat("Thread {0} was rmed.", threadId);
            inst.HandleThreadRemoval(threadId);
            threadMap.Remove(threadId);
        }
    }
}

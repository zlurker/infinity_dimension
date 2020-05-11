using System.Collections;
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

        //Debug.LogFormat("Thread ID {0} has called threadsplitter.", GetNodeThreadId());
        threadMap.Add(latestThread, new SplitterData());
        ProcessThreads(latestThread);
    }


    public override void ThreadZeroed(int parentThread) {
        (threadMap[parentThread] as SplitterData).numberOfLoops++;

        //Debug.Log("Thread End!");
        //if(GetCentralInst().GetActiveThread(lastChildThread).GetJointThread() == -1)
        currLoop++;
        GetCentralInst().SetTimerEventID(-1);
        ProcessThreads(parentThread);
    }

    /*public void PreProcessThread(int threadId) {

        

        ProcessThreads()

        if(pendingData.Count > 0) {
            List<int> pendingDataApplied = new List<int>();
            expediteThreads = pendingData.Count;

            foreach(var values in pendingData) {
                

                ProcessThreads(threadId);
                pendingDataApplied.Add(values.Key);
                expediteThreadsSpawned++;
            }

            // Only removes those that has been processed.
            for(int i = 0; i < pendingDataApplied.Count; i++)
                pendingData.Remove(pendingDataApplied[i]);

            // If new item was added during processing, it will not be removed. Hence, we will now apply all the other pending data.
            if(pendingData.Count > 0)
                PreProcessThread(threadId);
        } else
            ProcessThreads(threadId);
    }*/

    public void ProcessThreads(int threadId) {

        if(pendingData.ContainsKey(currLoop)) {
            for(int i = 0; i < pendingData[currLoop].Count; i++) {
                pendingData[currLoop][i].ApplyDataToTargetVariable(GetCentralInst());
                //Debug.Log("Data applied!");
            }

            pendingData.Remove(currLoop);
        }

        AbilityCentralThreadPool inst = GetCentralInst();
        SplitterData sData = threadMap[threadId] as SplitterData;

        //Debug.LogFormat("Thread id {0} currently {1}/{2}.", threadId, sData.numberOfLoops, inst.ReturnRuntimeParameter<int>(GetNodeId(), 0).v);

        if(sData.numberOfLoops < GetNodeVariable<int>("Number of Loops") || GetNodeVariable<int>("Number of Loops") == -1) {
            ChildThread trdInst = new ChildThread(GetNodeId(), threadId, this);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadDuplicatorTracker: ThreadMapDataBase {
    public int threadsDuplicated;

    public ThreadDuplicatorTracker(int totalThreadsDuplicated) {
        threadsDuplicated = totalThreadsDuplicated;
    }
}

public class ClientInput : NodeModifierBase, IInputCallback<int>, IOnSpawn {

    bool inputSet;

    private void Update() {

        int curr = GetNodeVariable<int>("Curr");
        int max = GetNodeVariable<int>("Max");

        if(curr < max)
            if(GetNodeThreadId() > -1) {

                Debug.Log("Current threadID: "+ GetNodeThreadId());
                int currThreadId = GetNodeThreadId();

                threadMap.Add(currThreadId, new ThreadDuplicatorTracker(max-curr));

                for(int i = curr; i < max; i++) {
                    //Debug.LogFormat("Curr: {0}, Max: {1}", i, max);
                    ChildThread threadInst = new ChildThread(GetNodeId(),currThreadId, this);
                    threadInst.SetNodeData(GetNodeId(), GetCentralInst().GetNodeBranchData(GetNodeId()));

                    int threadToUse = GetCentralInst().AddNewThread(threadInst);
                    SetVariable<int>(threadToUse, "Input Key", GetNodeVariable<int>("Input Key"));
                    
                }

                SetVariable<int>("Curr", max);                
            }
    }

    public void InputCallback(int callbackData) {
        inputSet = false;
        //Debug.Log("Input called");
        SetVariable<int>("Max", GetNodeVariable<int>("Max") + 1);
        //SetVariable<int>("Input Key");    
    }

    public override void NodeCallback() {
        destroyOverridenThreads = true;
        base.NodeCallback();

        if(IsClientPlayerUpdate()) {
            if(!inputSet)
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>("Input Key"), 1);

            inputSet = true;
        }
    }


    public override void ThreadZeroed(int parentThread) {
        base.ThreadZeroed(parentThread);

        ThreadDuplicatorTracker tMDB = threadMap[parentThread] as ThreadDuplicatorTracker;
        tMDB.threadsDuplicated--;

        if (tMDB.threadsDuplicated == 0) {
            threadMap.Remove(GetNodeThreadId());
            Debug.LogFormat("{0} removed from ThreadMap.", GetNodeThreadId());
            GetCentralInst().HandleThreadRemoval(GetNodeThreadId());
        }

    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0), VariableTypes.SIGNAL_ONLY),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Curr", 0), VariableTypes.HIDDEN),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Max", 0), VariableTypes.CLIENT_ACTIVATED,VariableTypes.HIDDEN)
        });
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadMapDataBase {

    public int totalThreadsSpawned;

    public ThreadMapDataBase() {
        totalThreadsSpawned = 0;
    }
}

public class NodeModifierBase : AbilityTreeNode {

    protected Dictionary<int, ThreadMapDataBase> threadMap = new Dictionary<int, ThreadMapDataBase>();
    protected int latestThread;
    protected bool destroyOverridenThreads;

    public override void NodeCallback() {
        base.NodeCallback();

        latestThread = GetNodeThreadId();

        // Sets it to be -ve 1 so current thread will not be overrode/deleted.
        if(!destroyOverridenThreads)
            SetNodeThreadId(-1);
    }

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        //Debug.Log("Thread end called");

        if(nT is ChildThread) {
            int parentThread = (nT as ChildThread).GetOriginalThread();

            Debug.Log("parentthread: " + parentThread);
            threadMap[parentThread].totalThreadsSpawned--;

            if(threadMap[parentThread].totalThreadsSpawned == 0)
                ThreadZeroed(parentThread);

            // Checks if node is already empty with no more threads.
            if(threadMap.Count == 0) {
                //Debug.Log("Threadmap empty. Setting node thread id to -1.");
                SetNodeThreadId(-1);
            }
        }
    }

    public virtual void ThreadZeroed(int parentThread) {

    }

    public void AddThread(int oT) {
        threadMap[oT].totalThreadsSpawned++;
    }
}

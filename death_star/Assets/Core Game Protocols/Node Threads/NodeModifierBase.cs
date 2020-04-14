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

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        //Debug.Log("Thread end called");

        if(nT is ChildThread) {
            int parentThread = (nT as ChildThread).GetOriginalThread();
            threadMap[parentThread].totalThreadsSpawned--;

            if(threadMap[parentThread].totalThreadsSpawned == 0) 
                ThreadZeroed(parentThread, threadId);            
            
            // Checks if node is already empty with no more threads.
            if(threadMap.Count == 0) {
                Debug.Log("Threadmap empty. Setting node thread id to -1.");
                SetNodeThreadId(-1);
            }
        }
    }

    public virtual void ThreadZeroed(int parentThread, int lastChildThread) {

    }

    public void AddThread(int oT) {
        threadMap[oT].totalThreadsSpawned++;
    }
}

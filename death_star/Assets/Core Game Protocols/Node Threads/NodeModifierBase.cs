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
        base.ThreadEndStartCallback(threadId);

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        if(nT is ChildThread) {
            int parentThread = (nT as ChildThread).GetOriginalThread();
            threadMap[parentThread].totalThreadsSpawned--;

            if(threadMap[parentThread].totalThreadsSpawned == 0)
                ThreadZeroed(parentThread);
        }
    }

    public virtual void ThreadZeroed(int parentThread) {
        threadMap.Remove(parentThread);
    }

    public void AddThread(int oT) {
        threadMap[oT].totalThreadsSpawned++;
    }
}

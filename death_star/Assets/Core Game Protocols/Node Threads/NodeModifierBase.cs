﻿using System.Collections;
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

    // Would cause null error if the thread doesn't exist anymore while referred back after TESC.
    protected bool destroyOverridenThreads;

    public override void NodeCallback() {
        base.NodeCallback();

        latestThread = GetNodeThreadId();

        // Sets it to be -ve 1 so current thread will not be deleted.
        if(!destroyOverridenThreads)
            SetNodeThreadId(-1);
    }

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = GetCentralInst();
        NodeThread nT = inst.GetActiveThread(threadId);

        //Debug.Log("Thread end called");


        int parentThread = nT.GetOriginalThread();

        if(parentThread > -1) {
            threadMap[parentThread].totalThreadsSpawned--;

            if(threadMap[parentThread].totalThreadsSpawned == 0)
                ThreadZeroed(parentThread);
        }
    }

    public virtual void ThreadZeroed(int parentThread) {

    }

    public void AddThread(int oT) {
        if(oT > -1)
            threadMap[oT].totalThreadsSpawned++;
    }
}

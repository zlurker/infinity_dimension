using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadMapDataBase {

    public int totalThreadsSpawned;

    public ThreadMapDataBase() {
        totalThreadsSpawned = 0;
    }
}

public class NodeModifierBaseThread: NodeThread {
    int givenId;
    NodeModifierBase nodeModifierBase;

    public NodeModifierBaseThread(int gId, NodeModifierBase nMB) {
        givenId = gId;
        nodeModifierBase = nMB;

        nodeModifierBase.AddThread(givenId);
    }

    public int GetGivenId() {
        return givenId;
    }

    public override NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NodeModifierBaseThread(givenId, nodeModifierBase);

        return null;
    }

    public override void OnThreadEnd() {
        nodeModifierBase.ThreadEndStartCallback(this);
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

    public virtual void ThreadEndStartCallback(NodeModifierBaseThread endedThread) {

        AbilityCentralThreadPool inst = GetCentralInst();

        //Debug.Log("Thread end called");

        int parentThread = endedThread.GetGivenId();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildThread : NodeThread {

    int originalThread;
    ISubNode subNode;

    public ChildThread(int sPt, int oT,ISubNode sN) : base(sPt) {
        originalThread = oT;
        subNode = sN;

        subNode.AddThread(originalThread);
    }

    public override NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new ChildThread(GetStartingPoint(), originalThread, subNode);

        return null;
    }

    public int GetOriginalThread() {
        return originalThread;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildThread : NodeThread {

    int originalThread;
    NodeModifierBase nMB;

    public ChildThread(int sPt, int oT, NodeModifierBase sN) : base(sPt) {
        originalThread = oT;
        nMB = sN;

        nMB.AddThread(originalThread);
    }

    public override NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new ChildThread(GetStartingPoint(), originalThread, nMB);

        return null;
    }

    public int GetOriginalThread() {
        return originalThread;
    }
}
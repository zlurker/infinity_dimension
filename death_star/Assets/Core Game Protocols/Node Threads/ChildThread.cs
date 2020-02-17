using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildThread : NodeThread {

    int originalThread;

    public ChildThread(int sPt, int oT) : base(sPt) {
        originalThread = oT;
    }

    public override NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new ChildThread(GetStartingPoint(), originalThread);

        return null;
    }

    public int GetOriginalThread() {
        return originalThread;
    }
}
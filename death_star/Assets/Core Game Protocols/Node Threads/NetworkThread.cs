using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkThread : NodeThread {

    int originalThread; 

    public NetworkThread(int sPt,int oT) : base(sPt) {
        originalThread = oT;
    }

    public override NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NetworkThread(GetStartingPoint(), originalThread);

        return null;
    }
}

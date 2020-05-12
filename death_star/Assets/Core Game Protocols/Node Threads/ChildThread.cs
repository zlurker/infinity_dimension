using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class NodeThread {

    int givenId;
    NodeModifierBase nMB;

    int currNode;

    // To be used for creation of new threads when it branches out.
    // generatedNodeTheads/possiblePaths.       
    protected int generatedNodeThreads;
    protected int possiblePaths;

    public NodeThread(int oT, NodeModifierBase sN) {
        givenId = oT;
        nMB = sN;

        if(nMB != null)
            nMB.AddThread(givenId);
    }

    public NodeModifierBase GetStartingPoint() {
        return nMB;
    }

    public void SetNodeData(int cN, int pS) {
        currNode = cN;
        SetPossiblePaths(pS);
    }

    public void SetPossiblePaths(int pS) {
        generatedNodeThreads = 0;
        possiblePaths = pS;
    }

    public int GetCurrentNodeID() {
        return currNode;
    }

    public int GetPossiblePaths() {
        return possiblePaths;
    }

    public int GetGivenId() {
        return givenId;
    }

    public NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NodeThread(givenId, nMB);

        return null;
    }
}*/
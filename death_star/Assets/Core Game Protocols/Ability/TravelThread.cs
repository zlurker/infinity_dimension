using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeThread {

    int threadId;
    int currNode;
    int startingPt;
    int currLoop;
    int loops;

    // To be used for creation of new threads when it branches out.
    // generatedNodeTheads/possiblePaths.       
    int generatedNodeThreads;
    int possiblePaths;

    // Used for nesting of threads.
    int parentThread;

    // To be used if thread overlaps with thread on the same node.
    int jointThread;

    public NodeThread(int sPt) {
        startingPt = sPt;
        loops = 1;
        parentThread = -1;
    }

    public void SetId(int id) {
        threadId = id;
    }

    public void JoinThread(int thread) {
        jointThread = thread;
    }

    public void SetNodeData(int cN, int pS) {
        generatedNodeThreads = 0;
        currNode = cN;
        possiblePaths = pS;
    }

    public int GetCurrentNodeID() {
        return currNode;
    }

    public bool CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return false;

        return true;
    }
}

public class TravelThread {

    public static EnhancedList<TravelThread> globalCentralList = new EnhancedList<TravelThread>();

    Variable[][] runtimeParameters;
    Type[] subclassTypes;

    int[] branchStartData;
    int[] nodeBranchingData;
    int[] nodeType;

    // Link to ability nodes.
    int abilityNodes;

    int centralId;

    EnhancedList<NodeThread> activeThreads;

    public void SetCentralData(int tId, Variable[][] rP, Type[] sT, int[] bSD, int[] nBD, int[] nT) {
        activeThreads = new EnhancedList<NodeThread>();

        centralId = tId;
        runtimeParameters = rP;
        subclassTypes = sT;
        branchStartData = bSD;
        nodeBranchingData = nBD;
        nodeType = nT;
    }

    public int GetNewThread(int startNode) {
        return activeThreads.Add(new NodeThread(startNode));
    }

    public void StartThreads() {

        for(int i = 0; i < branchStartData.Length; i++) {
            int threadId = GetNewThread(branchStartData[i]);
            UpdateThreadNodeData(threadId, branchStartData[i]);
        }
    }

    public void NodeVariableCallback(int threadId, int variableId, VariableAction vA) {

        //Debug.Log(runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[(int)vA].Length);
        //Debug.Log(runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[(int)vA][0][0]);

        for(int i = 0; i < runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[(int)vA].Length; i++) {

            bool createNew = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            int nodeId = runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[(int)vA][i][0];

            if(createNew)
                threadIdToUse = GetNewThread(nodeId);

            UpdateThreadNodeData(threadIdToUse, nodeId);
        }
    }

    public void UpdateThreadNodeData(int threadId, int node) {

        AbilityTreeNode inst = CreateNewNodeIfNull(node);
        int prevNodeId = activeThreads.l[threadId].GetCurrentNodeID();
        inst.SetNodeId(node);

        if(inst.GetNodeThreadId() > -1)
            activeThreads.l[threadId].JoinThread(inst.GetNodeThreadId());

        inst.SetNodeThreadId(threadId);       
        activeThreads.l[threadId].SetNodeData(node, nodeBranchingData[node]);

        inst.NodeCallback(prevNodeId, 0, 0);
    }

    public AbilityTreeNode CreateNewNodeIfNull(int nodeId) {

        if(!AbilityTreeNode.globalList.l[abilityNodes][nodeId]) {
            AbilityTreeNode.globalList.l[abilityNodes][nodeId] = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { subclassTypes[nodeId] });
            AbilityTreeNode inst = Spawner.GetCType<AbilityTreeNode>(AbilityTreeNode.globalList.l[abilityNodes][nodeId]);
            inst.SetNodeThreadId(-1);

            return inst;
        }
        return Spawner.GetCType<AbilityTreeNode>(AbilityTreeNode.globalList.l[abilityNodes][nodeId]);
    }
}

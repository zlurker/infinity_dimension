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
        currNode = sPt;
        currLoop = 0;
        loops = 1;

        jointThread = -1;
        parentThread = -1;
    }

    public NodeThread(int sPt, int l) {

        startingPt = sPt;
        currNode = sPt;
        currLoop = 0;
        loops = l;

        jointThread = -1;
        parentThread = -1;
    }

    public int GetStartingPoint() {
        return startingPt;
    }

    public void SetId(int id) {
        threadId = id;
    }

    public void JoinThread(int thread) {
        jointThread = thread;
    }

    public int GetJointThread() {
        return jointThread;
    }

    public void SetNodeData(int cN, int pS) {

        generatedNodeThreads = 0;
        jointThread = -1;
        currNode = cN;
        possiblePaths = pS;
    }

    public int GetCurrentNodeID() {
        return currNode;
    }

    public bool CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return true;

        return false;
    }

    public void IncrementCompletion() {
        currLoop++;
    }

    public bool IsThreadComplete() {
        if(currLoop < loops)
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

    public RuntimeParameters<T> ReturnVariable<T>(int node, int variable) {
        return runtimeParameters[node][variable].field as RuntimeParameters<T>;
    }

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

    public int AddNewThread(NodeThread inst) {
        return activeThreads.Add(inst);
    }

    public NodeThread GetActiveThread(int threadId) {
        return activeThreads.l[threadId];
    }

    public void StartThreads() {

        for(int i = 0; i < branchStartData.Length; i++) {
            int threadId = GetNewThread(branchStartData[i]);
            UpdateThreadNodeData(threadId, branchStartData[i]);
        }
    }

    public void SeeNodeThreadLoop(int threadId) {
        NodeThread inst = activeThreads.l[threadId];

        if(inst.IsThreadComplete()) {

            // When thread finishes looping, call the callback.
            CreateNewNodeIfNull(inst.GetStartingPoint()).ThreadEndStartCallback(threadId);

        } else {

            // Else, just continue looping.
            // Run modified UpdateThreadNodeData without joint and node setting.

            // Basically, it CANNOT override node data or be join together with other threads.
            // Maybe can solve it with preset variables within NodeThread, ie. allowJoin, allowNodeOverride
            Debug.Log("Loop detected on TT");
            int node = inst.GetStartingPoint();
            AbilityTreeNode treeNode = CreateNewNodeIfNull(node);
            activeThreads.l[threadId].SetNodeData(node, nodeBranchingData[node]);
            treeNode.NodeCallback(threadId);
        }
    }

    public void NodeVariableCallback<T>(int threadId, int variableId, T value) {

        int jointThreadId = activeThreads.l[threadId].GetJointThread();

        if(jointThreadId > -1)
            NodeVariableCallback<T>(jointThreadId, variableId, value);

        for(int i = 0; i < runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[1].Length; i++) {

            bool createNew = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            int nodeId = runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[1][i][0];

            if(createNew)
                threadIdToUse = GetNewThread(nodeId);

            else {
                //If no creation needed, means its the last.
                int node = activeThreads.l[threadId].GetCurrentNodeID();
                AbilityTreeNode inst = CreateNewNodeIfNull(node);

                // Checks if the original thread is equal to the NTID to make sure we only set the thread id once to default.
                if(inst.GetNodeThreadId() == threadId)
                    inst.SetNodeThreadId(-1);
            }

            ((RuntimeParameters<T>)runtimeParameters[nodeId][variableId].field).v = value;
            UpdateThreadNodeData(threadIdToUse, nodeId);
        }
    }

    public void UpdateThreadNodeData(int threadId, int node) {

        AbilityTreeNode inst = CreateNewNodeIfNull(node);
        int prevNodeId = activeThreads.l[threadId].GetCurrentNodeID();
        int existingThread = inst.GetNodeThreadId();

        activeThreads.l[threadId].SetNodeData(node, nodeBranchingData[node]);

        if(existingThread > -1)
            /*if(activeThreads.l[threadId].JoinThread() && activeThreads.l[existingThread].JoinThread()) */{
            Debug.LogFormat("Thread {0} trying to join existing Thread{1}", threadId, existingThread);
            activeThreads.l[threadId].JoinThread(existingThread);
        }

        inst.SetNodeThreadId(threadId);
        inst.NodeCallback(threadId);

        // Checks if node has no more output
        if(nodeBranchingData[node] == 0) {
            inst.SetNodeThreadId(-1);
            activeThreads.l[threadId].IncrementCompletion();
            SeeNodeThreadLoop(threadId);
        }
    }

    public AbilityTreeNode CreateNewNodeIfNull(int nodeId) {

        if(!AbilityTreeNode.globalList.l[abilityNodes][nodeId]) {
            AbilityTreeNode.globalList.l[abilityNodes][nodeId] = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { subclassTypes[nodeId] });
            AbilityTreeNode inst = Spawner.GetCType<AbilityTreeNode>(AbilityTreeNode.globalList.l[abilityNodes][nodeId]);

            inst.SetNodeThreadId(-1);
            inst.SetNodeId(nodeId);
            inst.SetCentralId(centralId);
            return inst;
        }

        return Spawner.GetCType<AbilityTreeNode>(AbilityTreeNode.globalList.l[abilityNodes][nodeId]);
    }
}

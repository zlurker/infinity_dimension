﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeThread {

    int currNode;
    int startingPt;

    // To be used for creation of new threads when it branches out.
    // generatedNodeTheads/possiblePaths.       
    protected int generatedNodeThreads;
    protected int possiblePaths;

    // To be used if thread overlaps with thread on the same node.
    int jointThread;
    bool allowJoin;

    // To be used to decide if thread is able to override node data.
    bool allowOverride;

    public NodeThread(int sPt) {

        startingPt = sPt;
        currNode = sPt;

        jointThread = -1;
        allowJoin = true;
        allowOverride = true;
    }

    public void SetJoin(bool value) {
        allowJoin = value;
    }

    public bool ReturnJoin() {
        return allowJoin;
    }

    public void SetOverride(bool value) {
        allowOverride = value;
    }

    public bool ReturnOverride() {
        return allowOverride;
    }

    public int GetStartingPoint() {
        return startingPt;
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

    public virtual NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NodeThread(startingPt);

        return null;
    }
}

public class TravelThread {

    public static EnhancedList<TravelThread> globalCentralList = new EnhancedList<TravelThread>();

    Variable[][] runtimeParameters;
    Type[] subclassTypes;

    int[] branchStartData;
    int[] nodeBranchingData;
    int[] nodeType;

    Dictionary<int, int> specialisedNodeData;

    // Link to ability nodes.
    int abilityNodes;

    int centralId;

    EnhancedList<NodeThread> activeThreads;

    public int GetSpecialisedNodeData(int threadId) {
        return specialisedNodeData[threadId];
    }

    public Variable ReturnVariable(int node, int variable) {
        return runtimeParameters[node][variable];
    }

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, int variable) {
        return runtimeParameters[node][variable].field as RuntimeParameters<T>;
    }

    public void SetCentralData(int tId, Variable[][] rP, Type[] sT, int[] bSD, int[] nBD, int[] nT,Dictionary<int,int> sND) {
        activeThreads = new EnhancedList<NodeThread>();

        centralId = tId;
        runtimeParameters = rP;
        subclassTypes = sT;
        branchStartData = bSD;
        nodeBranchingData = nBD;
        nodeType = nT;
        specialisedNodeData = sND;
    }

    public int GetNodeBranchData(int id) {
        return nodeBranchingData[id];
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

    public void NodeVariableCallback<T>(int threadId, int variableId, T value) {

        int jointThreadId = activeThreads.l[threadId].GetJointThread();

        if(jointThreadId > -1) 
            NodeVariableCallback<T>(jointThreadId, variableId, value);

        int currNode = activeThreads.l[threadId].GetCurrentNodeID();

        for(int i = 0; i < runtimeParameters[currNode][variableId].links[1].Length; i++) {

            NodeThread newThread = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            int nodeId = runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[i][0];

            if(newThread != null) {
                threadIdToUse = activeThreads.Add(newThread);
                Debug.LogFormat("{0} has been spawned by {1}, ischild: {2}", threadIdToUse, threadId, activeThreads.l[threadId] is ChildThread);
            } else {
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

        //Debug.LogFormat("{0} end. {1} length", threadId, runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[1].Length);
    }

    public void UpdateThreadNodeData(int threadId, int node) {

        AbilityTreeNode inst = CreateNewNodeIfNull(node);
        int prevNodeId = activeThreads.l[threadId].GetCurrentNodeID();
        int existingThread = inst.GetNodeThreadId();

        activeThreads.l[threadId].SetNodeData(node, nodeBranchingData[node]);

        if(existingThread > -1)
            if(activeThreads.l[threadId].ReturnJoin() && activeThreads.l[existingThread].ReturnJoin()) {
                Debug.LogFormat("Thread {0} trying to join existing Thread{1}", threadId, existingThread);
                activeThreads.l[threadId].JoinThread(existingThread);
            }

        if(activeThreads.l[threadId].ReturnOverride())
            inst.SetNodeThreadId(threadId);

        inst.NodeCallback(threadId);

        // Checks if node has no more output
        if(nodeBranchingData[node] == 0) {
            inst.SetNodeThreadId(-1);

            // Callback to start node.
            ThreadEndCallback(threadId);
        }
    }

    public void ThreadEndCallback(int threadId) {
        CreateNewNodeIfNull(activeThreads.l[threadId].GetStartingPoint()).ThreadEndStartCallback(threadId);
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
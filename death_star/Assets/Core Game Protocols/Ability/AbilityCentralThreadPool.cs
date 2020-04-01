﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityNodeNetworkData<T> : AbilityNodeNetworkData {

    public T value;

    public AbilityNodeNetworkData(int nId, int vId, T v) {
        nodeId = nId;
        varId = vId;
        value = v;

        dataType = typeof(T);
    }
}

public class AbilityNodeNetworkData {
    public int nodeId;
    public int varId;
    public Type dataType;
}

public class NodeThread {

    int currNode;
    int startingPt;

    // To be used for creation of new threads when it branches out.
    // generatedNodeTheads/possiblePaths.       
    protected int generatedNodeThreads;
    protected int possiblePaths;

    // To be used if thread overlaps with thread on the same node.
    int jointThread;

    public NodeThread(int sPt) {

        startingPt = sPt;
        currNode = -1;
        jointThread = -1;
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

public class AbilityCentralThreadPool : NetworkObject {

    public static EnhancedList<AbilityCentralThreadPool> globalCentralList = new EnhancedList<AbilityCentralThreadPool>();

    public AbilityCentralThreadPool() {
        playerCasted = 0;
    }

    public AbilityCentralThreadPool(int pId) {
        playerCasted = pId;
    }

    private Variable[][] runtimeParameters;
    private Type[] subclassTypes;

    private int[] branchStartData;
    private int[] nodeBranchingData;

    private Dictionary<int, int> specialisedNodeData;

    //private AbilityBooleanData booleanData;
    private bool[][] booleanData;

    // Link to ability nodes
    private int abilityNodes;

    private int playerCasted;

    // This thread's ID
    private int centralId;

    private List<AbilityNodeNetworkData> networkNodeData;

    // Current threads active
    private EnhancedList<NodeThread> activeThreads;

    #region Network-Related Code
    private int networkObjectId;
    private int instId;

    public void NetworkObjectCreationCallback(int networkObjId, int iId) {
        networkObjectId = networkObjId;
        instId = iId;
    }

    public int ReturnNetworkObjectId() {
        return networkObjectId;
    }

    public int ReturnInstId() {
        return instId;
    }
    #endregion

    public int GetAbilityNodeId() {
        return abilityNodes;
    }

    public int GetSpecialisedNodeData(int threadId) {
        return specialisedNodeData[threadId];
    }

    /*public Variable ReturnVariable(int node, int variable) {
        int[] varAddress = variableReference[node][variable];

        if(varAddress != null)
            return runtimeParameters[varAddress[0]][varAddress[1]];

        return runtimeParameters[node][variable];
    }*/

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, int variable) {
        return runtimeParameters[node][variable].field as RuntimeParameters<T>;
    }

    public void SetCentralData(int tId, int nId, Variable[][] rP, Type[] sT, int[] bSD, int[] nBD, Dictionary<int, int> sND, bool[][] aBD) {
        activeThreads = new EnhancedList<NodeThread>();

        centralId = tId;
        abilityNodes = nId;
        runtimeParameters = rP;
        subclassTypes = sT;
        branchStartData = bSD;
        nodeBranchingData = nBD;
        specialisedNodeData = sND;
        booleanData = aBD;

        networkNodeData = new List<AbilityNodeNetworkData>();
    }

    public int GetNodeBranchData(int id) {
        return nodeBranchingData[id];
    }

    public int GetNewThread(int startNode) {
        return activeThreads.Add(new NodeThread(startNode));
    }

    public bool[] GetNodeBoolValues(int id) {
        return booleanData[id];
    }

    public int GetPlayerId() {
        return playerCasted;
    }

    public int AddNewThread(NodeThread inst) {
        return activeThreads.Add(inst);
    }

    public NodeThread GetActiveThread(int threadId) {
        return activeThreads.l[threadId];
    }

    public void AddVariableNetworkData(AbilityNodeNetworkData aNND) {
        networkNodeData.Add(aNND);
    }

    /*public void SendVariableNetworkData() {
        UpdateAbilityDataEncoder inst = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.UPDATE_ABILITY_DATA] as UpdateAbilityDataEncoder;
        inst.SendVariableManifest(networkObjectId, instId, networkNodeData.ToArray());
        networkNodeData.Clear();
    }*/

    public AbilityNodeNetworkData[] GetVariableNetworkData() {
        AbilityNodeNetworkData[] data = networkNodeData.ToArray();
        networkNodeData.Clear();
        return data;
    }

    public void StartThreads() {

        for(int i = 0; i < branchStartData.Length; i++) {
            int threadId = GetNewThread(branchStartData[i]);
            UpdateThreadNodeData(threadId, branchStartData[i]);
        }
    }

    public void NodeVariableCallback<T>(int threadId, int variableId, T value) {

        if(threadId == -1)
            return;

        int currNode = activeThreads.l[threadId].GetCurrentNodeID();
        bool sharedNetworkData = false;

        if(LoadedData.GetVariableType(subclassTypes[currNode], variableId, VariableTypes.CLIENT_ACTIVATED))
            if(playerCasted != ClientProgram.clientId)
                return;
            else
                sharedNetworkData = true;

        if(LoadedData.GetVariableType(subclassTypes[currNode], variableId, VariableTypes.HOST_ACTIVATED))
            if(playerCasted != ClientProgram.hostId)
                return;
            else
                sharedNetworkData = true;

        if(sharedNetworkData)
            AddVariableNetworkData(new AbilityNodeNetworkData<T>(currNode, variableId, value));

        UpdateVariableData<T>(threadId, variableId, value);
    }

    public void UpdateVariableData<T>(int threadId, int variableId, T value) {

        //Debug.Log("ThreadId in loop:" + threadId);
        int jointThreadId = activeThreads.l[threadId].GetJointThread();
        int currNode = activeThreads.l[threadId].GetCurrentNodeID();


        for(int i = 0; i < runtimeParameters[currNode][variableId].links.Length; i++) {

            NodeThread newThread = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            int nodeId = runtimeParameters[currNode][variableId].links[i][0];
            int nodeVariableId = runtimeParameters[currNode][variableId].links[i][1];

            if(newThread != null) {
                threadIdToUse = activeThreads.Add(newThread);
                Debug.LogFormat("{0} has been spawned by {1}, ischild: {2}", threadIdToUse, threadId, activeThreads.l[threadId] is ChildThread);
            } else {
                //If no creation needed, means its the last.
                //int node = activeThreads.l[threadId].GetCurrentNodeID();
                AbilityTreeNode inst = CreateNewNodeIfNull(currNode);

                // Checks if the original thread is equal to the NTID to make sure we only set the thread id once to default.
                if(inst.GetNodeThreadId() == threadId)
                    inst.SetNodeThreadId(-1);
            }

            RuntimeParameters<T> paramInst = runtimeParameters[nodeId][nodeVariableId].field as RuntimeParameters<T>;

            if(paramInst != null) {
                paramInst.v = value;
                booleanData[nodeId][nodeVariableId] = false;
            }

            UpdateThreadNodeData(threadIdToUse, nodeId);
        }

        if(jointThreadId > -1)
            UpdateVariableData<T>(jointThreadId, variableId, value);
        //Debug.LogFormat("{0} end. {1} length", threadId, runtimeParameters[activeThreads.l[threadId].GetCurrentNodeID()][variableId].links[1].Length);
    }

    // Handles the aftermath of callback. Passes thread to another node.
    public void UpdateThreadNodeData(int threadId, int node) {

        AbilityTreeNode inst = CreateNewNodeIfNull(node);
        //int prevNodeId = activeThreads.l[threadId].GetCurrentNodeID();
        int existingThread = inst.GetNodeThreadId();

        activeThreads.l[threadId].SetNodeData(node, nodeBranchingData[node]);

        if(existingThread > -1) {
            //if(activeThreads.l[threadId].ReturnJoin() && activeThreads.l[existingThread].ReturnJoin()) {
            Debug.LogFormat("Thread {0} trying to join existing Thread{1}", threadId, existingThread);
            activeThreads.l[threadId].JoinThread(existingThread);
        }

        //if(activeThreads.l[threadId].ReturnOverride())
        inst.SetNodeThreadId(threadId);

        inst.NodeCallback(threadId);

        // Checks if node has no more output
        if(nodeBranchingData[node] == 0) {
            inst.SetNodeThreadId(-1);
            HandleThreadRemoval(threadId);
        }
    }

    public void HandleThreadRemoval(int threadId) {

        Debug.LogFormat("Thread {0} has ended operations.", threadId);
        // Callback to start node.

        CreateNewNodeIfNull(activeThreads.l[threadId].GetStartingPoint()).ThreadEndStartCallback(threadId);

        // Removes that thread.
        activeThreads.Remove(threadId);

        //Debug.LogFormat("{0} threadIdRemoved, 1st Element: {1}", threadId, activeThreads.ReturnActiveElementIndex()[0]);

        if(activeThreads.GetActiveElementsLength() == 0) {
            Debug.Log("All thread operations has ended.");
        }
    }

    public AbilityTreeNode CreateNewNodeIfNull(int nodeId) {

        if(!AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId]) {

            // Tries to convert type into a singleton to see if it exist.
            if(LoadedData.singletonList.ContainsKey(subclassTypes[nodeId]))
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId] = LoadedData.singletonList[subclassTypes[nodeId]] as AbilityTreeNode;

            if(AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId] == null) {
                SpawnerOutput sOInst = LoadedData.GetSingleton<Spawner>().CreateScriptedObject(subclassTypes[nodeId]);
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId] = sOInst.script as AbilityTreeNode;
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId].SetSourceObject(sOInst);

                // Changes its name
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId].name = networkObjectId.ToString() + '/' + nodeId.ToString();

                // Adds it to root
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId].transform.SetParent(AbilityTreeNode.globalList.l[abilityNodes].abilityNodeRoot);
            }

            AbilityTreeNode inst = AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId];

            inst.SetNodeThreadId(-1);
            inst.SetNodeId(nodeId);
            inst.SetCentralId(centralId);
            return inst;
        }

        return AbilityTreeNode.globalList.l[abilityNodes].abiNodes[nodeId];
    }

    public void RenameAllNodes() {
        for (int i =0; i < AbilityTreeNode.globalList.l[abilityNodes].abiNodes.Length; i++) 
            if (AbilityTreeNode.globalList.l[abilityNodes].abiNodes[i] != null)
            AbilityTreeNode.globalList.l[abilityNodes].abiNodes[i].name = networkObjectId.ToString() + '/' + i.ToString();        
    }
}

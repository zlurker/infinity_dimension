using System.Collections;
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
        jointThread = -1;
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

    public virtual NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NodeThread(startingPt);

        return null;
    }
}

public class AbilityCentralThreadPool : NetworkObject, IRPGeneric {

    public static EnhancedList<AbilityCentralThreadPool> globalCentralList = new EnhancedList<AbilityCentralThreadPool>();

    public AbilityCentralThreadPool() {
        playerCasted = 0;
    }

    public AbilityCentralThreadPool(int pId) {
        playerCasted = pId;
    }

    private Variable[][] runtimeParameters;
    private Type[] subclassTypes;

    private int[] nodeBranchingData;

    //private AbilityBooleanData booleanData;
    private bool[][] booleanData;

    private int[][] autoManagedVar;

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

    public Variable ReturnVariable(int node, int variable) {
        return runtimeParameters[node][variable];
    }

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, int variable) {
        return runtimeParameters[node][variable].field as RuntimeParameters<T>;
    }

    public Variable ReturnVariable(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return runtimeParameters[node][variable];
    }

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return runtimeParameters[node][variable].field as RuntimeParameters<T>;
    }

    public void SetCentralData(int tId, int nId, Variable[][] rP, Type[] sT, int[] nBD, bool[][] aBD, int[][] amVar) {
        activeThreads = new EnhancedList<NodeThread>();

        centralId = tId;
        abilityNodes = nId;
        runtimeParameters = rP;
        subclassTypes = sT;
        nodeBranchingData = nBD;
        booleanData = aBD;
        autoManagedVar = amVar;

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

    public AbilityNodeNetworkData[] GetVariableNetworkData() {
        AbilityNodeNetworkData[] data = networkNodeData.ToArray();
        networkNodeData.Clear();
        return data;
    }

    public void StartThreads() {
        int lastNodeId = runtimeParameters.Length - 1;
        int threadId = GetNewThread(lastNodeId);

        activeThreads.l[threadId].SetNodeData(lastNodeId, nodeBranchingData[lastNodeId]);
        NodeVariableCallback<int>(threadId, 0);
    }

    public void UpdateVariableValue<T>(int threadId, int variableId, T value) {

        if(threadId == -1)
            return;

        int nodeId = activeThreads.l[threadId].GetCurrentNodeID();
        RuntimeParameters<T> paramInst = runtimeParameters[nodeId][variableId].field as RuntimeParameters<T>;

        if(paramInst != null)
            paramInst.v = value;

        else if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.INTERCHANGEABLE)) {
            string varName = runtimeParameters[nodeId][variableId].field.n;
            int[][] links = runtimeParameters[nodeId][variableId].links;
            runtimeParameters[nodeId][variableId] = new Variable(new RuntimeParameters<T>(varName, value), links);
        }
    }

    public void NodeVariableCallback<T>(int threadId, int variableId) {

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


        if(sharedNetworkData) {
            RuntimeParameters<T> paramInst = runtimeParameters[currNode][variableId].field as RuntimeParameters<T>;
            AddVariableNetworkData(new AbilityNodeNetworkData<T>(currNode, variableId, paramInst.v));
        }

        UpdateVariableData<T>(threadId, variableId);
    }

    public void UpdateVariableData<T>(int threadId, int variableId) {

        if(threadId == -1)
            return;

        int jointThreadId = activeThreads.l[threadId].GetJointThread();
        int currNode = activeThreads.l[threadId].GetCurrentNodeID();
        int[][] links = runtimeParameters[currNode][variableId].links;

        RuntimeParameters<T> originalParamInst = runtimeParameters[currNode][variableId].field as RuntimeParameters<T>;

        for(int i = 0; i < links.Length; i++) {

            NodeThread newThread = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            int nodeId = links[i][0];
            int nodeVariableId = links[i][1];
            int linkType = links[i][2];

            if(newThread != null) {
                threadIdToUse = activeThreads.Add(newThread);
                //newThread.SetSources(currNode,vSource);
                Debug.LogFormat("{0} has been spawned by {1}, ischild: {2}", threadIdToUse, threadId, activeThreads.l[threadId] is ChildThread);

            } else {
                //If no creation needed, means its the last.
                //int node = activeThreads.l[threadId].GetCurrentNodeID();
                AbilityTreeNode currNodeInst = CreateNewNodeIfNull(currNode);

                // Checks if the original thread is equal to the NTID to make sure we only set the thread id once to default.
                if(currNodeInst.GetNodeThreadId() == threadId)
                    currNodeInst.SetNodeThreadId(-1);

                //activeThreads.l[threadId].SetSources(currNode,vSource);
            }


            RuntimeParameters<T> targetParamInst = runtimeParameters[nodeId][nodeVariableId].field as RuntimeParameters<T>;

            Debug.LogFormat("{0}, original. {1}, target", typeof(T), runtimeParameters[nodeId][nodeVariableId].field.t);

            if(targetParamInst != null) {
                switch((LinkMode)linkType) {
                    case LinkMode.NORMAL:
                        targetParamInst.v = originalParamInst.v;
                        booleanData[nodeId][nodeVariableId] = false;

                        
                        Debug.LogFormat("Var set by {0},{1}",nodeId,nodeVariableId);
                       
                        break;
                }
            }

            AbilityTreeNode nextNodeInst = CreateNewNodeIfNull(nodeId);

            int existingThread = nextNodeInst.GetNodeThreadId();

            nextNodeInst.SetNodeThreadId(threadIdToUse);

            if(existingThread > -1) {
                Debug.LogFormat("Thread {0} trying to join existing Thread{1}", threadIdToUse, existingThread);
                activeThreads.l[threadIdToUse].JoinThread(existingThread);
            }

            activeThreads.l[threadIdToUse].SetNodeData(nodeId, nodeBranchingData[nodeId]);
            nextNodeInst.NodeCallback(threadIdToUse);

            for(int j = 0; j < autoManagedVar[nodeId].Length; j++)
                runtimeParameters[nodeId][autoManagedVar[nodeId][j]].field.RunGenericBasedOnRP<int[]>(this, new int[] { nodeId, autoManagedVar[nodeId][j] });

            // Checks if node has no more output
            if(nodeBranchingData[nodeId] == 0) {
                nextNodeInst.SetNodeThreadId(-1);
                HandleThreadRemoval(threadIdToUse);
            }
        }

        if(jointThreadId > -1)
            UpdateVariableData<T>(jointThreadId, variableId);
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
        for(int i = 0; i < AbilityTreeNode.globalList.l[abilityNodes].abiNodes.Length; i++)
            if(AbilityTreeNode.globalList.l[abilityNodes].abiNodes[i] != null)
                AbilityTreeNode.globalList.l[abilityNodes].abiNodes[i].name = networkObjectId.ToString() + '/' + i.ToString();
    }


    // This should be ran with curr node rather than thread.
    public void RunAccordingToGeneric<T, P>(P arg) {
        int[] nodeCBInfo = (int[])(object)arg;

        RuntimeParameters<T> rp = runtimeParameters[nodeCBInfo[0]][nodeCBInfo[1]].field as RuntimeParameters<T>;

        AbilityTreeNode inst = CreateNewNodeIfNull(nodeCBInfo[0]);

        NodeVariableCallback<T>(inst.GetNodeThreadId(), nodeCBInfo[1]);
    }
}

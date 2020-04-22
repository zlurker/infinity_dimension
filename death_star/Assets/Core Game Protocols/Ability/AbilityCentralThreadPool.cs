using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum NETWORK_CLIENT_ELIGIBILITY {
    GRANTED, DENIED, LOCAL_HOST
}

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

public class AbilityCentralThreadPool : NetworkObject, IRPGeneric, ITimerCallback {

    public static EnhancedList<AbilityCentralThreadPool> globalCentralList = new EnhancedList<AbilityCentralThreadPool>();
    public static EnhancedList<List<int>> globalCentralClusterList = new EnhancedList<List<int>>();

    public AbilityCentralThreadPool() {
        playerCasted = 0;
        InitialiseCentralVariables();
    }

    public AbilityCentralThreadPool(int pId) {
        playerCasted = pId;
        InitialiseCentralVariables();
    }

    void InitialiseCentralVariables() {
        networkNodeData = new Dictionary<int, List<AbilityNodeNetworkData>>();
        activeThreads = new EnhancedList<NodeThread>();
        timerEventId = -1;
        networkObjectId = -1;
        instId = -1;
    }

    private Variable[][] runtimeParameters;
    private Type[] subclassTypes;
    private Transform abilityNodeRoot;
    private AbilityTreeNode[] nodes;

    private int[] nodeBranchingData;

    //private AbilityBooleanData booleanData;
    private bool[][] booleanData;

    private int[][] autoManagedVar;

    private int playerCasted;

    // This thread's ID
    private int centralId;

    private int timerEventId;

    private Dictionary<int, List<AbilityNodeNetworkData>> networkNodeData;

    private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> onChanged;
    private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> onGet;

    private Dictionary<int, HashSet<Tuple<int, int>>> sharedInstance;

    private int centralClusterId;
    private int clusterPos;

    //private List<AbilityNodeNetworkData> networkNodeData;

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

    public AbilityTreeNode GetNode(int id) {
        return nodes[id];
    }

    public Transform GetAbilityRoot() {
        return abilityNodeRoot;
    }

    // Base method to get variables
    public Variable ReturnVariable(int node, int variable) {

        AbilityTreeNode rootNode = GetRootReferenceNode(node);
        bool notInstanced = LoadedData.GetVariableType(subclassTypes[node], variable, VariableTypes.NON_INSTANCED);

        if(rootNode != null && !notInstanced)
            return GetRootReferenceCentral(node).ReturnVariable(rootNode.GetNodeId(), variable);

        return runtimeParameters[node][variable];
    }

    public Variable ReturnVariable(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return ReturnVariable(node, variable);
    }

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return ReturnRuntimeParameter<T>(node, variable);
    }

    public RuntimeParameters<T> ReturnRuntimeParameter<T>(int node, int variable) {
        return ReturnVariable(node, variable).field as RuntimeParameters<T>;
    }

    public void SetCentralData(int tId, Variable[][] rP, Type[] sT, int[] nBD, bool[][] aBD, int[][] amVar, int cId) {

        abilityNodeRoot = new GameObject(tId.ToString()).transform;

        centralId = tId;
        runtimeParameters = rP;
        subclassTypes = sT;
        nodeBranchingData = nBD;
        booleanData = aBD;
        autoManagedVar = amVar;
        centralClusterId = cId;
        nodes = new AbilityTreeNode[rP.Length];

        onChanged = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
        onGet = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
        sharedInstance = new Dictionary<int, HashSet<Tuple<int, int>>>();
    }

    public void AddOnChanged(Tuple<int, int> key, Tuple<int, int> value) {

        if(!onChanged.ContainsKey(key))
            onChanged.Add(key, new HashSet<Tuple<int, int>>());

        if(!onChanged[key].Contains(value))
            onChanged[key].Add(value);
    }

    public void AddSharedInstance(int key, Tuple<int, int> value) {
        if(!sharedInstance.ContainsKey(key))
            sharedInstance.Add(key, new HashSet<Tuple<int, int>>());

        if(!sharedInstance[key].Contains(value))
            sharedInstance[key].Add(value);
    }

    public void RemoveSharedInstance(int key, Tuple<int, int> value) {
        if(sharedInstance.ContainsKey(key))
            if(sharedInstance[key].Contains(value))
                sharedInstance[key].Remove(value);
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

    public void SetNodeBoolValue(bool value, int node, int var) {
        booleanData[node][var] = value;
    }

    public int GetClusterID() {
        return centralClusterId;
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

    public void SetTimerEventID(int id) {
        timerEventId = id;
    }

    public void AddVariableNetworkData(AbilityNodeNetworkData aNND) {
        //Debug.Log("Variable Data added.");

        if(timerEventId > -1) {
            //Debug.Log("Timer extended.");
            LoadedData.GetSingleton<Timer>().UpdateEventStartTime(timerEventId, Time.realtimeSinceStartup);
        } else {
            //Debug.Log("New timer added.");
            timerEventId = LoadedData.GetSingleton<Timer>().CreateNewTimerEvent(0.05f, this);
            networkNodeData.Add(timerEventId, new List<AbilityNodeNetworkData>());
        }

        networkNodeData[timerEventId].Add(aNND);
    }

    public void CallOnTimerEnd(int eventId) {
        UpdateAbilityDataEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.UPDATE_ABILITY_DATA] as UpdateAbilityDataEncoder;
        Debug.Log("Send data worth " + networkNodeData.Count);

        AbilityNodeNetworkData[] data = networkNodeData[eventId].ToArray();
        networkNodeData.Remove(eventId);

        encoder.SendVariableManifest(this, data);

        if(timerEventId == eventId)
            timerEventId = -1;
    }

    public void StartThreads() {
        int lastNodeId = runtimeParameters.Length - 1;
        int threadId = GetNewThread(lastNodeId);

        activeThreads.l[threadId].SetNodeData(lastNodeId, nodeBranchingData[lastNodeId]);
        NodeVariableCallback<int>(threadId, 0);
    }

    public AbilityCentralThreadPool GetRootReferenceCentral(int nodeId) {
        Tuple<int, int> reference = nodes[nodeId].GetReference();
        return globalCentralList.l[reference.Item1];
    }

    public AbilityTreeNode GetRootReferenceNode(int nodeId) {

        if(nodes[nodeId] == null)
            return null;

        Tuple<int, int> reference = nodes[nodeId].GetReference();

        // Returns null if this is the root.
        if(reference == null || (reference.Item1 == centralId && reference.Item2 == nodeId))
            return null;

        AbilityCentralThreadPool refCentral = globalCentralList.l[reference.Item1];
        return refCentral.GetNode(reference.Item2);
    }

    public NETWORK_CLIENT_ELIGIBILITY CheckEligibility(int nodeId, int variableId) {

        if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.CLIENT_ACTIVATED)) {
            if(playerCasted != ClientProgram.clientId)
                return NETWORK_CLIENT_ELIGIBILITY.DENIED;
            else
                return NETWORK_CLIENT_ELIGIBILITY.GRANTED;
        }

        if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.HOST_ACTIVATED))
            if(ClientProgram.hostId != ClientProgram.clientId)
                return NETWORK_CLIENT_ELIGIBILITY.DENIED;
            else
                return NETWORK_CLIENT_ELIGIBILITY.GRANTED;

        return NETWORK_CLIENT_ELIGIBILITY.LOCAL_HOST;
    }

    public void UpdateVariableValue<T>(int nodeId, int variableId, T value, bool runValueChanged = true) {

        AbilityTreeNode refNode = GetRootReferenceNode(nodeId);
        bool notInstanced = LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.NON_INSTANCED);

        // If reference is not empty, redirects it to change that variable instead.
        if(refNode != null && !notInstanced) {
            GetRootReferenceCentral(nodeId).UpdateVariableValue<T>(refNode.GetNodeId(), variableId, value, runValueChanged);
            return;
        }

        RuntimeParameters<T> paramInst = runtimeParameters[nodeId][variableId].field as RuntimeParameters<T>;
        T[] valuePair = new T[2];

        if(paramInst != null) {
            valuePair[0] = paramInst.v;
            valuePair[1] = value;

            paramInst.v = value;
        } else if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.INTERCHANGEABLE)) {
            string varName = runtimeParameters[nodeId][variableId].field.n;
            int[][] links = runtimeParameters[nodeId][variableId].links;

            //Debug.LogFormat("Var changed from {0} to {1}", runtimeParameters[nodeId][variableId].field.t, typeof(T));
            runtimeParameters[nodeId][variableId] = new Variable(new RuntimeParameters<T>(varName, value), links);

            valuePair[0] = value;
            valuePair[1] = value;
        } else
            runValueChanged = false;

        // Does run value stuff here.
        if(runValueChanged) {
            // Needs rework.
            Tuple<int, int> id = Tuple.Create<int, int>(nodeId, variableId);

            //Debug.Log(nodeId);
            //Debug.Log(variableId);

            if(onChanged.ContainsKey(id)) {

                foreach(var changeCallback in onChanged[id]) {
                    //Debug.Log(changeCallback.Item1);
                    //Debug.Log(changeCallback.Item2);

                    OnValueChange valChangeNode = nodes[changeCallback.Item2] as OnValueChange;
                    valChangeNode.HandleSettingOnChange<T>(valuePair);
                }

                onChanged.Remove(id);
            }
        }
    }

    public void NodeVariableCallback<T>(int threadId, int variableId) {

        if(threadId == -1)
            return;

        int currNode = activeThreads.l[threadId].GetCurrentNodeID();
        NETWORK_CLIENT_ELIGIBILITY nCE = CheckEligibility(currNode, variableId);

        //Debug.Log(nCE);

        switch(nCE) {
            case NETWORK_CLIENT_ELIGIBILITY.GRANTED:
                RuntimeParameters<T> paramInst = runtimeParameters[currNode][variableId].field as RuntimeParameters<T>;
                AddVariableNetworkData(new AbilityNodeNetworkData<T>(currNode, variableId, paramInst.v));
                break;
            case NETWORK_CLIENT_ELIGIBILITY.DENIED:
                return;
        }

        UpdateVariableData<T>(threadId, variableId);
    }

    public void UpdateVariableData<T>(int threadId, int variableId, RuntimeParameters<T> var = null) {

        if(threadId == -1)
            return;

        int jointThreadId = activeThreads.l[threadId].GetJointThread();
        int currNode = activeThreads.l[threadId].GetCurrentNodeID();
        int[][] links = runtimeParameters[currNode][variableId].links;

        if(var == null)
            var = runtimeParameters[currNode][variableId].field as RuntimeParameters<T>;

        for(int i = 0; i < links.Length; i++) {

            NodeThread newThread = activeThreads.l[threadId].CreateNewThread();
            int threadIdToUse = threadId;
            //Debug.LogFormat("Current info: CurrNode{0}, CurrVar{1}, CurrLink{2}, CurrLinkLen{3}",currNode,variableId,i,links[i].Length);
            int nodeId = links[i][0];
            int nodeVariableId = links[i][1];
            int linkType = links[i][2];

            if(newThread != null) {
                threadIdToUse = activeThreads.Add(newThread);
                //newThread.SetSources(currNode,vSource);
                //Debug.LogFormat("{0} has been spawned by {1}, ischild: {2}", threadIdToUse, threadId, activeThreads.l[threadId] is ChildThread);

            } else {
                //If no creation needed, means its the last.
                //int node = activeThreads.l[threadId].GetCurrentNodeID();
                AbilityTreeNode currNodeInst = CreateNewNodeIfNull(currNode);

                // Checks if the original thread is equal to the NTID to make sure we only set the thread id once to default.
                if(currNodeInst.GetNodeThreadId() == threadId)
                    currNodeInst.SetNodeThreadId(-1);

                //activeThreads.l[threadId].SetSources(currNode,vSource);
            }

            activeThreads.l[threadIdToUse].SetNodeData(nodeId, nodeBranchingData[nodeId]);

            RuntimeParameters<T> targetParamInst = runtimeParameters[nodeId][nodeVariableId].field as RuntimeParameters<T>;

            switch((LinkMode)linkType) {
                case LinkMode.NORMAL:
                    //Debug.Log(originalParamInst.v);
                    booleanData[nodeId][nodeVariableId] = false;
                    UpdateVariableValue<T>(nodeId, nodeVariableId, var.v);
                    break;
            }


            AbilityTreeNode nextNodeInst = CreateNewNodeIfNull(nodeId);

            int existingThread = nextNodeInst.GetNodeThreadId();

            nextNodeInst.SetNodeThreadId(threadIdToUse);

            if(existingThread > -1) {
                Debug.LogFormat("Thread {0} trying to join existing Thread{1}", threadIdToUse, existingThread);
                activeThreads.l[threadIdToUse].JoinThread(existingThread);
            }


            nextNodeInst.NodeCallback(threadIdToUse);

            // Automatically callback all auto managed nodes.
            for(int j = 0; j < autoManagedVar[nodeId].Length; j++)
                runtimeParameters[nodeId][autoManagedVar[nodeId][j]].field.RunGenericBasedOnRP<int[]>(this, new int[] { nodeId, autoManagedVar[nodeId][j] });

            // Checks if node has no more output
            if(nodeBranchingData[nodeId] == 0) {
                nextNodeInst.SetNodeThreadId(-1);
                HandleThreadRemoval(threadIdToUse);
            }
        }

        // Updates the other instances.
        if(sharedInstance.ContainsKey(currNode))
            foreach(var inst in sharedInstance[currNode]) {
                Debug.LogFormat("Central {0} Node {1} is a instance to be set.", inst.Item1, inst.Item2);
                AbilityTreeNode selectedNode = globalCentralList.l[inst.Item1].GetNode(inst.Item2);

                globalCentralList.l[inst.Item1].UpdateVariableData<T>(selectedNode.GetNodeThreadId(), variableId, var);
                //AbilityTreeNode.globalList.l[inst.Item1].abiNodes[inst.Item2].SetVariable<T>(variableId, var.v, VariableSetMode.LOCAL);
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

        if(!nodes[nodeId]) {

            // Tries to convert type into a singleton to see if it exist.
            if(LoadedData.singletonList.ContainsKey(subclassTypes[nodeId]))
                nodes[nodeId] = LoadedData.singletonList[subclassTypes[nodeId]] as AbilityTreeNode;

            if(nodes[nodeId] == null) {
                SpawnerOutput sOInst = LoadedData.GetSingleton<Spawner>().CreateScriptedObject(subclassTypes[nodeId]);
                nodes[nodeId] = sOInst.script as AbilityTreeNode;
                nodes[nodeId].SetSourceObject(sOInst);

                // Changes its name
                nodes[nodeId].name = networkObjectId.ToString() + '/' + nodeId.ToString();

                // Adds it to root
                nodes[nodeId].transform.SetParent(abilityNodeRoot);
            }

            AbilityTreeNode inst = nodes[nodeId];

            inst.SetNodeThreadId(-1);
            inst.SetNodeId(nodeId);
            inst.SetCentralId(centralId);
            return inst;
        }

        return nodes[nodeId];
    }

    public void RenameAllNodes() {
        for(int i = 0; i < nodes.Length; i++)
            if(nodes[i] != null)
                nodes[i].name = networkObjectId.ToString() + '/' + i.ToString();
    }


    // This should be ran with curr node rather than thread.
    public void RunAccordingToGeneric<T, P>(P arg) {
        int[] nodeCBInfo = (int[])(object)arg;

        AbilityTreeNode inst = CreateNewNodeIfNull(nodeCBInfo[0]);
        NodeVariableCallback<T>(inst.GetNodeThreadId(), nodeCBInfo[1]);
    }
}

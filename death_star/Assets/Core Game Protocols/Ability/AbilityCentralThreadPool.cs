﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum NETWORK_CLIENT_ELIGIBILITY {
    GRANTED, DENIED, LOCAL_HOST
}

public enum ON_VARIABLE_CATERGORY {
    ON_CHANGED, ON_CALLED
}

public class NetworkVariablesData {
    public int nodeCallbackCount;
    public int[] networkVariables;

    public NetworkVariablesData(int[] nwVar) {
        networkVariables = nwVar;
    }

}

public class AbilityNodeNetworkData<T> : AbilityNodeNetworkData {

    public T value;

    public AbilityNodeNetworkData(int nId, int vId, T v, int vSC) {
        nodeId = nId;
        varId = vId;
        value = v;
        variableSetCount = vSC;

        dataType = typeof(T);
    }

    public AbilityNodeNetworkData(int nId, int vId, T v) {
        nodeId = nId;
        varId = vId;
        value = v;

        dataType = typeof(T);
    }

    public override void ApplyDataToTargetVariable(AbilityCentralThreadPool central) {
        //Debug.LogFormat("Data applied: {0} to: {1}", value, central.GetNode(nodeId));
        central.UpdateVariableValue<T>(nodeId, varId, value, false);
        central.UpdateVariableData<T>(nodeId, varId);
    }
}

public class AbilityNodeNetworkData {
    public int nodeId;
    public int varId;
    public int variableSetCount;
    public Type dataType;

    public virtual void ApplyDataToTargetVariable(AbilityCentralThreadPool central) {
    }
}

public class NodeThread {

    int currNode;
    protected int threadChannel;

    // To be used for creation of new threads when it branches out.
    // generatedNodeTheads/possiblePaths.       
    protected int generatedNodeThreads;
    protected int possiblePaths;

    public NodeThread(int tC) {
        currNode = -1;
        threadChannel = tC;
    }

    public void SetNodeData(int cN, int pS) {
        currNode = cN;
        SetPossiblePaths(pS);
    }

    public void SetPossiblePaths(int pS) {
        generatedNodeThreads = 0;
        possiblePaths = pS;
    }

    public int GetThreadChannel() {
        return threadChannel;
    }

    public int GetCurrentNodeID() {
        return currNode;
    }

    public int GetPossiblePaths() {
        return possiblePaths;
    }

    public virtual NodeThread CreateNewThread() {
        generatedNodeThreads++;

        if(possiblePaths > generatedNodeThreads)
            return new NodeThread(threadChannel);

        return null;
    }

    public virtual void OnThreadEnd() {

    }
}


public class AbilityCentralThreadPool : IRPGeneric {

    //public static EnhancedList<AbilityCentralThreadPool> globalCentralList = new EnhancedList<AbilityCentralThreadPool>();

    public AbilityCentralThreadPool() {
        playerId = -1;
        InitialiseCentralVariables();
    }

    public AbilityCentralThreadPool(int pId) {
        playerId = pId;
        InitialiseCentralVariables();
    }

    void InitialiseCentralVariables() {
        networkNodeData = new List<AbilityNodeNetworkData>();
        activeThreads = new EnhancedList<NodeThread>();
        //networkObjectId = -1;
        //instId = -1;
    }

    //private Variable[][] runtimeParameters;
    private RuntimeParameters[][] runtimeParameters;
    private int[][][][][] linkMap;
    private Type[] subclassTypes;
    private Transform abilityNodeRoot;
    private AbilityTreeNode[] nodes;

    private int[] nodeBranchingData;
    
    //private AbilityBooleanData booleanData;
    private bool[][] booleanData;

    private int[][] autoManagedVar;
    private int playerId;

    private int castingPlayer;

    // This thread's ID
    private int centralId;

    private bool markPending;

    private Dictionary<int, NetworkVariablesData> networkVariableData;
    private List<AbilityNodeNetworkData> networkNodeData;

    private Dictionary<Tuple<int, int, int>, AbilityNodeNetworkData> pendingApplyData;

    //private Dictionary<int, HashSet<Tuple<int, int, int>>> instanceUpdateVarDataCallback;

    //private Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, HashSet<Tuple<int, int, int>>>> onCallbacks;
    private Dictionary<int, Tuple<int, int, int>> instancedNodes;


    // 1st tuple for target node/var, 2nd for types for this target, 3rd for 
    // This is generated by Manager.
    //private Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>> targettedNodes;

    // Current threads active
    private EnhancedList<NodeThread> activeThreads;

    public void AddPendingData(AbilityNodeNetworkData nodeNetworkData) {

        // Applies it if it matches node current status
        //Debug.LogFormat("TID {0}, TVSC {1}, GVSC {2}", nodes[nodeNetworkData.nodeId].GetNodeThreadId(), networkVariableData[nodeNetworkData.nodeId].nodeCallbackCount, nodeNetworkData.variableSetCount);
        if(nodes[nodeNetworkData.nodeId] != null)
            if(nodes[nodeNetworkData.nodeId].GetNodeThreadId() > -1 && networkVariableData[nodeNetworkData.nodeId].nodeCallbackCount == nodeNetworkData.variableSetCount) {
                //Debug.LogWarning("Pending data applied. #1");
                nodeNetworkData.ApplyDataToTargetVariable(this);
                return;
            }

        // If not, add this to pending list.
        //Debug.Log("Data added to " + Tuple.Create(nodeNetworkData.nodeId, nodeNetworkData.varId, nodeNetworkData.variableSetCount));
        pendingApplyData.Add(Tuple.Create(nodeNetworkData.nodeId, nodeNetworkData.varId, nodeNetworkData.variableSetCount), nodeNetworkData);
    }

    public AbilityTreeNode GetNode(int id) {
        return CreateNewNodeIfNull(id);
    }

    public Transform GetAbilityRoot() {
        return abilityNodeRoot;
    }

    public int[][] GetVariableLinks(int channel, int node, int variable) {
        return linkMap[channel][node][variable];
    }

    /*// Base method to get variables.
    public Variable ReturnVariable(int node, int variable) {

        
            return GetRootReferenceCentral(node).ReturnVariable(instancedNodes[node].Item3, variable);

        return runtimeParameters[node][variable];
    }

    public Variable ReturnVariable(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return ReturnVariable(node, variable);
    }*/

    public RuntimeParameters ReturnRuntimeParameter(int node, string vName) {
        int variable = LoadedData.loadedParamInstances[subclassTypes[node]].variableAddresses[vName];
        return ReturnRuntimeParameter(node, variable);
    }

    public RuntimeParameters ReturnRuntimeParameter(int node, int variable) {
        if(CheckIfReferenced(node, variable))
            return GetRootReferenceCentral(node).ReturnRuntimeParameter(instancedNodes[node].Item3, variable);

        return runtimeParameters[node][variable];
    }

    public int ReturnPlayerCasted() {
        return castingPlayer;
    }
    public int ReturnCentralId() {
        return centralId;
    }

    public void SetCentralData(int cP, int tId, RuntimeParameters[][] rP, int[][][][][] lM,Type[] sT, int[] nBD, bool[][] aBD, int[][] amVar, Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>> oVC, Dictionary<int, int[]> nwVD) {

        abilityNodeRoot = new GameObject(tId.ToString()).transform;
        //Debug.Log("Ability created.");

        castingPlayer = cP;
        centralId = tId;
        runtimeParameters = rP;
        linkMap = lM;
        subclassTypes = sT;
        nodeBranchingData = nBD;
        booleanData = aBD;
        autoManagedVar = amVar;
        nodes = new AbilityTreeNode[rP.Length];
        networkVariableData = new Dictionary<int, NetworkVariablesData>();

        foreach(var nodeNetworkData in nwVD)
            networkVariableData.Add(nodeNetworkData.Key, new NetworkVariablesData(nodeNetworkData.Value));

        //targettedNodes = new Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>>(oVC);

        pendingApplyData = new Dictionary<Tuple<int, int, int>, AbilityNodeNetworkData>();
        instancedNodes = new Dictionary<int, Tuple<int, int, int>>();
        //onCallbacks = new Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, HashSet<Tuple<int, int, int>>>>();
        //instanceUpdateVarDataCallback = new Dictionary<int, HashSet<Tuple<int, int, int>>>();
    }

    public Tuple<int, int, int> GetInstanceReference(int nodeId) {

        if(instancedNodes.ContainsKey(nodeId))
            return instancedNodes[nodeId];

        return null;
    }

    public void CopyNodeVariables(int nodeId, int playerId, int centralId, int targetNodeId) {

        for(int i = 0; i < runtimeParameters[nodeId].Length; i++)
            runtimeParameters[nodeId][i] = AbilitiesManager.aData[playerId].playerSpawnedCentrals.GetElementAt(centralId).runtimeParameters[targetNodeId][i].ReturnNewRuntimeParamCopy();
    }

    public void InstanceNode(int nodeId, Tuple<int, int, int> refNode) {
        if(!instancedNodes.ContainsKey(nodeId)) {
            instancedNodes.Add(nodeId, refNode);
            return;
        }

        instancedNodes[nodeId] = refNode;
        //updateNodes = false;

        /*if(updateNodes) {
            AbilityCentralThreadPool centralRoot;

            // Removes old link
            /*if(instancedNodes[nodeId] != null) {
                centralRoot = GetRootReferenceCentral(nodeId);
                centralRoot.RemoveQuickReference(instancedNodes[nodeId].Item2, instancedNodes[nodeId]);
            }

            // Adds new link


            // Quick references variable on called so we can call them quickly.
            centralRoot = GetRootReferenceCentral(nodeId);
            centralRoot.AddQuickReference(refNode.Item2, Tuple.Create(castingPlayer, centralId, nodeId));
        }*/

    }

    /*public void AddQuickReference(int key, Tuple<int, int, int> value) {

        // Handles instancing

        if(!onCallbacks.ContainsKey(key))
            onCallbacks.Add(key, new Dictionary<ON_VARIABLE_CATERGORY, HashSet<Tuple<int, int, int>>>());

        if(AbilitiesManager.GetAssetData(value.Item1).playerSpawnedCentrals.l[value.Item2].targettedNodes.ContainsKey(value.Item3))
            foreach(var item in AbilitiesManager.GetAssetData(value.Item1).playerSpawnedCentrals.l[value.Item2].targettedNodes[value.Item3]) {
                if(!onCallbacks[key].ContainsKey(item.Key))
                    onCallbacks[key].Add(item.Key, new HashSet<Tuple<int, int, int>>());

                if(!onCallbacks[key][item.Key].Contains(value))
                    onCallbacks[key][item.Key].Add(value);
            }

        /*if(!sharedInstance.ContainsKey(key))
            sharedInstance.Add(key, new HashSet<Tuple<int, int, int>>());

        if(!sharedInstance[key].Contains(value))
            sharedInstance[key].Add(value);
    }

    public void RemoveQuickReference(int key, Tuple<int, int, int> value) {
        //if(sharedInstance.ContainsKey(key))
        //if(sharedInstance[key].Contains(value))
        // sharedInstance[key].Remove(value);

        if(onCallbacks.ContainsKey(key))
            foreach(var item in onCallbacks[key])
                if(item.Value.Contains(value))
                    item.Value.Remove(value);
    }*/

    /*public void AddCalledCallback(int nodeId, Tuple<int, int, int> address) {
        if(!instanceUpdateVarDataCallback.ContainsKey(nodeId))
            instanceUpdateVarDataCallback.Add(nodeId, new HashSet<Tuple<int, int, int>>());

        if(!instanceUpdateVarDataCallback[nodeId].Contains(address))
            instanceUpdateVarDataCallback[nodeId].Add(address);
    }

    public void RemoveCalledCallback(int nodeId, Tuple<int, int, int> address) {
        if(instanceUpdateVarDataCallback.ContainsKey(nodeId))
            if(instanceUpdateVarDataCallback[nodeId].Contains(address))
                instanceUpdateVarDataCallback[nodeId].Remove(address);
    }*/

    public int GetNodeBranchData(int id) {
        return nodeBranchingData[id];
    }

    public int GetNewThread() {
        return activeThreads.Add(new NodeThread(0));
    }

    public bool[] GetNodeBoolValues(int id) {
        return booleanData[id];
    }

    public void SetNodeBoolValue(bool value, int node, int var) {
        booleanData[node][var] = value;
    }

    public int GetPlayerId() {
        return playerId;
    }

    public int AddNewThread(NodeThread inst) {
        return activeThreads.Add(inst);
    }

    public NodeThread GetActiveThread(int threadId) {
        return activeThreads.l[threadId];
    }

    /*public void SetTimerEventID(int id) {
        timerEventId = id;
    }*/

    public void AddVariableNetworkData(AbilityNodeNetworkData aNND) {
        //Debug.Log("Variable Data added.");

        /*if(timerEventId > -1) {
            //Debug.Log("Timer extended.");
            LoadedData.GetSingleton<Timer>().UpdateEventStartTime(timerEventId, Time.realtimeSinceStartup);
        } else {
            //Debug.Log("New timer added.");
            timerEventId = LoadedData.GetSingleton<Timer>().CreateNewTimerEvent(0.05f, this);
            networkNodeData.Add(timerEventId, new List<AbilityNodeNetworkData>());
        }*/

        if(!markPending) {
            markPending = true;
            LoadedData.GetSingleton<AbilityNetworkDataCompiler>().pendingData.Add(this);
        }


        //Debug.LogFormat("Central {0},{1} variable added..", castingPlayer, centralId);
        networkNodeData.Add(aNND);
    }

    public void CompileVariableNetworkData() {
        UpdateAbilityDataEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.UPDATE_ABILITY_DATA] as UpdateAbilityDataEncoder;


        //Debug.LogFormat("Data compiling.");      
        AbilityNodeNetworkData[] data = networkNodeData.ToArray();
        encoder.SendVariableManifest(this, data);
        markPending = false;
        networkNodeData.Clear();
    }

    /*public void CallOnTimerEnd(int eventId) {
        
        //Debug.Log("Sending data");

        
        networkNodeData.Remove(eventId);

        Debug.LogFormat("Central {0},{1} event ID {2} ended. Sending data to manifest.", castingPlayer, centralId, eventId);
        

        if(timerEventId == eventId)
            timerEventId = -1;
    }*/

    public void StartThreads() {

        //Debug.Log("Threads started!");

        int lastNodeId = runtimeParameters.Length - 2;

        // Direct node callback. It will auto handle creation of new threads ect.
        int threadId = GetNewThread();

        activeThreads.l[threadId].SetNodeData(lastNodeId, nodeBranchingData[lastNodeId]);
        CreateNewNodeIfNull(lastNodeId).SetNodeThreadId(threadId);

        //Debug.Log("Total links: " + runtimeParameters[lastNodeId][0].links.Length);
        UpdateVariableData<int>(lastNodeId, 0);
        //CreateNewNodeIfNull(lastNodeId).NodeCallback();
    }

    public bool CheckIfReferenced(int nodeId, int variableId) {
        if(nodes[nodeId] == null)
            return false;

        bool notInstanced = LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.NON_INSTANCED);

        if(notInstanced || !instancedNodes.ContainsKey(nodeId) || instancedNodes[nodeId] == null)
            return false;

        return true;
    }

    public AbilityCentralThreadPool GetRootReferenceCentral(int nodeId) {

        if(!instancedNodes.ContainsKey(nodeId))
            return this;

        Tuple<int, int, int> reference = instancedNodes[nodeId];

        if(reference != null)
            return AbilitiesManager.aData[reference.Item1].playerSpawnedCentrals.GetElementAt(reference.Item2);
        else
            return this;
    }

    public AbilityTreeNode GetRootReferenceNode(int nodeId) {

        if(nodes[nodeId] == null)
            return null;

        if(!instancedNodes.ContainsKey(nodeId))
            return GetRootReferenceCentral(nodeId).GetNode(nodeId);

        Tuple<int, int, int> reference = instancedNodes[nodeId];

        if(reference != null)
            return GetRootReferenceCentral(nodeId).GetNode(reference.Item3);
        else
            return GetRootReferenceCentral(nodeId).GetNode(nodeId);
    }

    /*public NETWORK_CLIENT_ELIGIBILITY CheckEligibility(int nodeId, int variableId) {

        if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.CLIENT_ACTIVATED)) {
            if(playerId != ClientProgram.clientId)
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
    }*/

    public void UpdateVariableValue<T>(int nodeId, int variableId, T value, bool runNetworkCode = true, bool runValueChanged = true) {

        bool reference = CheckIfReferenced(nodeId, variableId);

        // If reference is not empty, redirects it to change that variable instead.
        if(reference) {
            Tuple<int, int, int> refLink = instancedNodes[nodeId];

            //Debug.LogFormat("Var set, central {0}, node {1}, var {2}, value {3}", refLink.Item1, refLink.Item2, variableId, value);
            GetRootReferenceCentral(nodeId).UpdateVariableValue<T>(refLink.Item3, variableId, value, runValueChanged);
            return;
        }

        if(runNetworkCode) {

            if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.NETWORK)) {
                AbilityNodeNetworkData dataPacket = new AbilityNodeNetworkData<T>(nodeId, variableId, value, networkVariableData[nodeId].nodeCallbackCount);
                //Debug.Log("Input is been sent out.");
                //INodeNetworkPoint nwPointInst = nodes[progenitorData[nodeId]] as INodeNetworkPoint;
                //Debug.Log(nodes[progenitorData[nodeId]]);
                //nwPointInst.ModifyDataPacket(dataPacket);
                AddVariableNetworkData(dataPacket);
            }
            /*NETWORK_CLIENT_ELIGIBILITY nCE = CheckEligibility(nodeId, variableId);

            switch(nCE) {
                case NETWORK_CLIENT_ELIGIBILITY.GRANTED:
                    //Debug.Log("Data point going");
                    //Debug.LogFormat("Data going to be applied to: {0} to: {1}", value, GetNode(nodeId));
                    AbilityNodeNetworkData dataPacket = new AbilityNodeNetworkData<T>(nodeId, variableId, value, networkVariableData[nodeId].nodeCallbackCount);
                    //INodeNetworkPoint nwPointInst = nodes[progenitorData[nodeId]] as INodeNetworkPoint;
                    //Debug.Log(nodes[progenitorData[nodeId]]);
                    //nwPointInst.ModifyDataPacket(dataPacket);
                    AddVariableNetworkData(dataPacket);
                    break;

                case NETWORK_CLIENT_ELIGIBILITY.DENIED:
                    return;
            }*/
        }

        // Does run value stuff here.
        /*if(runValueChanged) {

            int totalOnCalled = RunTargettedNodes<T>(nodeId, variableId, ON_VARIABLE_CATERGORY.ON_CHANGED, value);

            if(onCallbacks.ContainsKey(nodeId))
                if(onCallbacks[nodeId].ContainsKey(ON_VARIABLE_CATERGORY.ON_CHANGED))
                    foreach(var id in onCallbacks[nodeId][ON_VARIABLE_CATERGORY.ON_CHANGED]) {
                        AbilityCentralThreadPool centralInst = AbilitiesManager.aData[id.Item1].playerSpawnedCentrals.GetElementAt(id.Item2);
                        totalOnCalled += centralInst.RunTargettedNodes<T>(id.Item3, variableId, ON_VARIABLE_CATERGORY.ON_CALLED, value);
                    }

            if(totalOnCalled > 0)
                return;
        }*/

        RuntimeParameters<T> paramInst = runtimeParameters[nodeId][variableId] as RuntimeParameters<T>;
        bool varWasSet = false;

        if(paramInst != null) {
            paramInst.v = value;
            booleanData[nodeId][variableId] = false;
            varWasSet = true;

        } else if(LoadedData.GetVariableType(subclassTypes[nodeId], variableId, VariableTypes.INTERCHANGEABLE)) {
            string varName = runtimeParameters[nodeId][variableId].n;
            //int[][] links = runtimeParameters[nodeId][variableId].links;

            //Debug.LogFormat("Var changed from {0} to {1}", runtimeParameters[nodeId][variableId].field.t, typeof(T));
            runtimeParameters[nodeId][variableId] = new RuntimeParameters<T>(varName, value);
            booleanData[nodeId][variableId] = false;
            varWasSet = true;
        }

        if(varWasSet && runValueChanged) {
            //Debug.Log(value);
            IOnVariableSet oVS = CreateNewNodeIfNull(nodeId) as IOnVariableSet;

            if(oVS != null)
                oVS.OnVariableSet(variableId);
        }
    }

    /*public void NodeVariableCallback<T>(int threadId, int variableId) {

        if(threadId == -1)
            return;

        int currNode = activeThreads.l[threadId].GetCurrentNodeID();
        NETWORK_CLIENT_ELIGIBILITY nCE = CheckEligibility(currNode, variableId);


        switch(nCE) {
            case NETWORK_CLIENT_ELIGIBILITY.GRANTED:
                //Debug.Log("Curr Node sent out: " + currNode);
                RuntimeParameters<T> paramInst = runtimeParameters[currNode][variableId].field as RuntimeParameters<T>;

                AbilityNodeNetworkData dataPacket =new AbilityNodeNetworkData<T>(currNode, variableId, paramInst.v);
                INodeNetworkPoint nwPointInst = nodes[progenitorData[currNode]] as INodeNetworkPoint;
                nwPointInst.ModifyDataPacket(dataPacket);
                AddVariableNetworkData(dataPacket);
                break;

            case NETWORK_CLIENT_ELIGIBILITY.DENIED:
                return;
        }

        UpdateVariableData<T>(threadId, variableId);
    }*/

    public void UpdateVariableData<T>(int currNode, int variableId, int threadId = -1, RuntimeParameters<T> var = null, bool runOnCalled = true) {

        //if(threadId == -1)
        //return;

        //int currNode = activeThreads.l[threadId].GetCurrentNodeID();

        //if(CheckEligibility(currNode, variableId) == NETWORK_CLIENT_ELIGIBILITY.DENIED)
        //return;

        if(var == null)
            var = ReturnRuntimeParameter(currNode, variableId) as RuntimeParameters<T>;
        //Debug.LogFormat("Curr updatevardata: {0},{1},{2}", castingPlayer, centralId, currNode);

        if(threadId == -1)
            threadId = nodes[currNode].GetNodeThreadId();

        //Debug.LogFormat("Variable it has: {0}, threadID {1}, currNode {2}, varId{3}", var.v ,threadId , currNode, variableId);


        if(threadId > -1) {

            int[][] links = linkMap[activeThreads.l[threadId].GetThreadChannel()][currNode][variableId];
            int currPossiblePaths = activeThreads.l[threadId].GetPossiblePaths();

            for(int i = 0; i < links.Length; i++) {

                int nodeId = links[i][0];
                int nodeVariableId = links[i][1];
                int linkType = links[i][2];
                int threadIdToUse = threadId;
                NodeThread newThread = activeThreads.l[threadId].CreateNewThread();

                if(newThread != null)
                    threadIdToUse = activeThreads.Add(newThread);
                else {
                    AbilityTreeNode currNodeInst = CreateNewNodeIfNull(currNode);

                    if(currNodeInst.GetNodeThreadId() == threadIdToUse)
                        currNodeInst.SetNodeThreadId(-1);
                }

                activeThreads.l[threadIdToUse].SetNodeData(nodeId, nodeBranchingData[nodeId]);

                AbilityTreeNode nextNodeInst = CreateNewNodeIfNull(nodeId);

                int existingThread = nextNodeInst.GetNodeThreadId();

                if(existingThread > -1)
                    HandleThreadRemoval(existingThread);
                //activeThreads.l[threadIdToUse](existingThread);

                //Debug.LogFormat("Thread travelling from {0} to {1} with variable {2}", nodes[currNode], nextNodeInst, var.v);
                nextNodeInst.SetNodeThreadId(threadIdToUse);

                /*if(CheckIfReferenced(nodeId, nodeVariableId)) {

                    // Only adds calledcallback if there is a need to.
                    if(nodeBranchingData[nodeId] > 0)
                        GetRootReferenceCentral(nodeId).AddCalledCallback(instancedNodes[nodeId].Item3, Tuple.Create(castingPlayer, centralId, nodeId));

                    GetRootReferenceCentral(nodeId).UpdateLinkEndPointData<T>(instancedNodes[nodeId].Item3, nodeVariableId, linkType, var, runOnCalled);
                } else
                    UpdateLinkEndPointData<T>(nodeId, nodeVariableId, linkType, var, runOnCalled);*/

                /*if(runOnCalled) {

                    int totalOnCalled = RunTargettedNodes<T>(nodeId, nodeVariableId, ON_VARIABLE_CATERGORY.ON_CALLED, var.v);

                    // Runs other instances OVC too
                    if(onCallbacks.ContainsKey(nodeId))
                        foreach(var id in onCallbacks[nodeId][ON_VARIABLE_CATERGORY.ON_CALLED]) {
                            AbilityCentralThreadPool centralInst = AbilitiesManager.aData[id.Item1].playerSpawnedCentrals.GetElementAt(id.Item2);
                            totalOnCalled += centralInst.RunTargettedNodes<T>(id.Item3, nodeVariableId, ON_VARIABLE_CATERGORY.ON_CALLED, var.v);
                        }

                    if(totalOnCalled > 0)
                        return;
                }*/

                if((LinkMode)linkType == LinkMode.NORMAL)
                    UpdateVariableValue<T>(nodeId, nodeVariableId, var.v);

                if(networkVariableData.ContainsKey(nodeId)) {
                    networkVariableData[nodeId].nodeCallbackCount++;


                    for(int j = 0; j < networkVariableData[nodeId].networkVariables.Length; j++) {
                        int networkVarId = networkVariableData[nodeId].networkVariables[j];
                        Tuple<int, int, int> pendingDataKey = Tuple.Create(nodeId, networkVarId, networkVariableData[nodeId].nodeCallbackCount);
                        //Debug.LogWarning("Pending data waiting #2, " + pendingDataKey);

                        if(pendingApplyData.ContainsKey(pendingDataKey)) {
                            pendingApplyData[pendingDataKey].ApplyDataToTargetVariable(this);
                        }
                    }
                }

                CreateNewNodeIfNull(nodeId).NodeCallback();

                // Automatically callback all auto managed nodes.
                for(int j = 0; j < autoManagedVar[nodeId].Length; j++)
                    // Callback those that are not blocked.
                    if(!booleanData[nodeId][autoManagedVar[nodeId][j]])
                        runtimeParameters[nodeId][autoManagedVar[nodeId][j]].RunGenericBasedOnRP<int[]>(this, new int[] { nodeId, autoManagedVar[nodeId][j] });

                if(nodeBranchingData[nodeId] == 0)
                    HandleThreadRemoval(threadIdToUse);
            }
        }

        // Updates all marked instance.
        /*if(instanceUpdateVarDataCallback.ContainsKey(currNode)) {

            HashSet<Tuple<int, int, int>> nodeDataToRm = new HashSet<Tuple<int, int, int>>();

            foreach(var item in instanceUpdateVarDataCallback[currNode].ToArray()) {
                AbilityCentralThreadPool centralInst = AbilitiesManager.aData[item.Item1].playerSpawnedCentrals.GetElementAt(item.Item2);
                centralInst.UpdateVariableData<T>(item.Item3, variableId, -1, var, runOnCalled);

                // Checks if node still has any thread on it. If not, removes it.
                if(centralInst.GetNode(item.Item3).GetNodeThreadId() == -1)
                    nodeDataToRm.Add(item);
            }

            instanceUpdateVarDataCallback[currNode].ExceptWith(nodeDataToRm);
        }*/

        // Needs to be removed from instance side.
        //markedNodes.Remove(currNode);

        // Updates the other instances.
        /*if(sharedInstance.ContainsKey(currNode))
            foreach(var inst in sharedInstance[currNode]) {
                //Debug.LogFormat("Central {0} Node {1} is a instance to be set.", inst.Item1, inst.Item2);
                AbilityCentralThreadPool centralInst = AbilitiesManager.aData[inst.Item1].playerSpawnedCentrals.GetElementAt(inst.Item2);

                centralInst.UpdateVariableData<T>(centralInst.GetNode(inst.Item3).GetNodeThreadId(), variableId, var, runOnCalled);
            }*/
    }

    /*public void UpdateLinkEndPointData<T>(int nodeId, int variableId, int linkType, RuntimeParameters<T> var = null, bool runOnCalled = true) {
        // Does instancing shit here

        if(runOnCalled) {

            int totalOnCalled = RunTargettedNodes<T>(nodeId, variableId, ON_VARIABLE_CATERGORY.ON_CALLED, var.v);

            // Runs other instances OVC too
            if(onCallbacks.ContainsKey(nodeId))
                foreach(var id in onCallbacks[nodeId][ON_VARIABLE_CATERGORY.ON_CALLED]) {
                    AbilityCentralThreadPool centralInst = AbilitiesManager.aData[id.Item1].playerSpawnedCentrals.GetElementAt(id.Item2);
                    totalOnCalled += centralInst.RunTargettedNodes<T>(id.Item3, variableId, ON_VARIABLE_CATERGORY.ON_CALLED, var.v);
                }

            if(totalOnCalled > 0)
                return;
        }

        if((LinkMode)linkType == LinkMode.NORMAL)
            UpdateVariableValue<T>(nodeId, variableId, var.v);

        if(networkVariableData.ContainsKey(nodeId)) {
            networkVariableData[nodeId].nodeCallbackCount++;


            for(int i = 0; i < networkVariableData[nodeId].networkVariables.Length; i++) {
                int networkVarId = networkVariableData[nodeId].networkVariables[i];
                Tuple<int, int, int> pendingDataKey = Tuple.Create(nodeId, networkVarId, networkVariableData[nodeId].nodeCallbackCount);
                //Debug.LogWarning("Pending data waiting #2, " + pendingDataKey);

                if(pendingApplyData.ContainsKey(pendingDataKey)) {
                    pendingApplyData[pendingDataKey].ApplyDataToTargetVariable(this);
                }
            }
        }

        CreateNewNodeIfNull(nodeId).NodeCallback();

        // Automatically callback all auto managed nodes.
        for(int j = 0; j < autoManagedVar[nodeId].Length; j++)
            // Callback those that are not blocked.
            if(!booleanData[nodeId][autoManagedVar[nodeId][j]])
                runtimeParameters[nodeId][autoManagedVar[nodeId][j]].field.RunGenericBasedOnRP<int[]>(this, new int[] { nodeId, autoManagedVar[nodeId][j] });
    }*/

    /*public int RunTargettedNodes<T>(int node, int variable, ON_VARIABLE_CATERGORY category, T value) {
        int targetInCatergory = 0;

        if(targettedNodes.ContainsKey(node))
            if(targettedNodes[node].ContainsKey(category))
                foreach(var vCLoop in targettedNodes[node][category]) {

                    if(variable == vCLoop.Key)
                        targetInCatergory += vCLoop.Value.Count;

                    foreach(int vC in vCLoop.Value) {
                        IOnVariableSet nodeInst = GetNode(vC) as IOnVariableSet;
                        nodeInst.CentralCallback<T>(value, node, variable, 0);
                    }
                }

        return targetInCatergory;
    }*/


    public void HandleThreadRemoval(int threadId) {

        //int cNode = activeThreads.l[threadId].GetCurrentNodeID();
        //int sPoint = activeThreads.l[threadId].GetStartingPoint();
        //Debug.LogFormat("Thread {0} ending at {1}", threadId, CreateNewNodeIfNull(activeThreads.l[threadId].GetCurrentNodeID()).GetType());
        if(threadId == -1)
            return;

        //Debug.Log("Thread removed " + threadId);
        AbilityTreeNode threadNode = CreateNewNodeIfNull(activeThreads.l[threadId].GetCurrentNodeID());

        // Only sets it to 0 if the current thread on node is the same as thread to be removed.
        if(threadNode.GetNodeThreadId() == threadId)
            threadNode.SetNodeThreadId(-1);

        activeThreads.l[threadId].OnThreadEnd();
        //CreateNewNodeIfNull(activeThreads.l[threadId].GetStartingPoint()).ThreadEndStartCallback(threadId);

        activeThreads.Remove(threadId);
        //Debug.LogFormat("Thread {0} has been removed.", threadId);
    }

    public AbilityTreeNode CreateNewNodeIfNull(int nodeId) {

        //Debug.Log(nodeId);
        if(!nodes[nodeId]) {

            // Tries to convert type into a singleton to see if it exist.
            if(LoadedData.singletonList.ContainsKey(subclassTypes[nodeId]))
                nodes[nodeId] = LoadedData.singletonList[subclassTypes[nodeId]] as AbilityTreeNode;

            if(nodes[nodeId] == null) {
                SpawnerOutput sOInst = LoadedData.GetSingleton<Spawner>().CreateScriptedObject(subclassTypes[nodeId]);
                nodes[nodeId] = sOInst.script as AbilityTreeNode;
                nodes[nodeId].SetSourceObject(sOInst);

                // Changes its name
                nodes[nodeId].name = castingPlayer.ToString() + '/' + centralId.ToString() + '/' + nodeId.ToString();

                // Adds it to root
                nodes[nodeId].transform.SetParent(abilityNodeRoot);
            }

            AbilityTreeNode inst = nodes[nodeId];

            inst.SetNodeThreadId(-1);
            inst.SetNodeId(nodeId);
            inst.SetCentralId(castingPlayer, centralId);

            IOnNodeInitialised oNNInst = inst as IOnNodeInitialised;

            if(oNNInst != null)
                oNNInst.OnNodeInitialised();

            return inst;
        }

        return nodes[nodeId];
    }


    // This should be ran with curr node rather than thread.
    public void RunAccordingToGeneric<T, P>(P arg) {
        int[] nodeCBInfo = (int[])(object)arg;

        AbilityTreeNode inst = CreateNewNodeIfNull(nodeCBInfo[0]);
        UpdateVariableData<T>(inst.GetNodeId(), nodeCBInfo[1]);
    }
}

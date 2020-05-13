using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LinkMode {
    NORMAL, SIGNAL
}

public enum VariableSetMode {
    LOCAL, INSTANCE
}

public class AbilityNodeHolder {
    public Transform abilityNodeRoot;
    public AbilityTreeNode[] abiNodes;

    public AbilityNodeHolder(string id, AbilityTreeNode[] nodes) {
        abilityNodeRoot = new GameObject(id).transform;
        abiNodes = nodes;
    }
}

public class AbilityTreeNode : MonoBehaviour {

    // Given node ID.
    private int nodeId;

    private int castingPlayerId;
    private int centralThreadId;
    private int nodeThreadId;
    private bool selfRef;
    private SpawnerOutput sourceObject;

    /*public virtual void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

    }*/

    public virtual void ConstructionPhase(AbilityData data) {

    }

    public int GetNodeThreadId() {
        return nodeThreadId;
    }

    public void SetNodeThreadId(int id) {
        nodeThreadId = id;
    }

    public int GetNodeId() {
        return nodeId;
    }

    public void SetNodeId(int id) {
        nodeId = id;
    }

    public int GetCastingPlayerId() {
        return castingPlayerId;
    }

    public int GetCentralId() {
        return centralThreadId;
    }

    public void SetCentralId(int cP, int id) {
        castingPlayerId = cP;
        centralThreadId = id;
    }

    public SpawnerOutput GetSourceObject() {
        return sourceObject;
    }

    public void SetSourceObject(SpawnerOutput srcObject) {
        sourceObject = srcObject;
    }

    public virtual void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("This Node", null), VariableTypes.NON_INSTANCED));
    }

    public virtual void NodeCallback() {

        AbilityTreeNode refNode = GetNodeVariable<AbilityTreeNode>("This Node");

        if(refNode != null) 
            InstanceThisNode(refNode);
            //if(refNode.GetType().IsSubclassOf(GetType()) || (GetType().IsSubclassOf(refNode.GetType())) || refNode.GetType() == GetType()) {
        if(CheckIfVarRegionBlocked("This Node"))
            GetCentralInst().UpdateVariableData<AbilityTreeNode>(nodeThreadId, 0, new RuntimeParameters<AbilityTreeNode>(this));
    }

    public void InstanceThisNode(AbilityTreeNode parent) {

        // Closes this game object as it is just a instance of another object.
        gameObject.SetActive(false);
       
        // Creates new link
        Tuple<int, int, int> refNode = parent.GetCentralInst().GetInstanceReference(parent.GetNodeId());


        // If null, means the parent node is the root node.
        if(refNode == null)
            refNode = Tuple.Create<int, int, int>(parent.GetCentralInst().ReturnPlayerCasted(),parent.GetCentralId(),parent.GetNodeId());

        // Instances node on our side
        GetCentralInst().InstanceNode(nodeId, refNode);        
    }

    public bool CheckIfVarRegionBlocked(params string[] target) {
        bool[] nodeBoolValues = GetCentralInst().GetNodeBoolValues(nodeId);

        for(int i = 0; i < target.Length; i++)
            if(nodeBoolValues[GetVariableId(target[i])])
                return false;


        return true;
    }

    public AbilityCentralThreadPool GetCentralInst() {
        //Debug.Log("CPID: " +castingPlayerId);
        //Debug.Log("CTID: " + centralThreadId);
        return AbilitiesManager.aData[castingPlayerId].playerSpawnedCentrals.GetElementAt(centralThreadId);
    }

    public bool IsClientPlayerUpdate() {
        return GetCentralInst().GetPlayerId() == ClientProgram.clientId;
    }

    public T GetNodeVariable<T>(string var) {
        return GetCentralInst().ReturnRuntimeParameter<T>(nodeId, var).v;
    }

    public void SetVariable<T>(int varId, T value) {
        GetCentralInst().UpdateVariableValue(nodeId, varId, value);
        GetCentralInst().UpdateVariableData<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(int threadId, string varName) {
        int varId = GetVariableId(varName);
        GetCentralInst().UpdateVariableData<T>(threadId, varId);
    }

    public void SetVariable<T>(int threadId, string varName, T value) {
        int varId = GetVariableId(varName);
        GetCentralInst().UpdateVariableValue(nodeId, varId, value);
        GetCentralInst().UpdateVariableData<T>(threadId, varId);
    }

    public void SetVariable<T>(string varName, T value) {
        int varId = GetVariableId(varName);
        GetCentralInst().UpdateVariableValue(nodeId, varId, value);
        GetCentralInst().UpdateVariableData<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(string varName) {
        GetCentralInst().UpdateVariableData<T>(nodeThreadId, GetVariableId(varName));
    }

    public int GetVariableId(string varName) {
        return LoadedData.loadedParamInstances[GetType()].variableAddresses[varName];
    }

    public virtual SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
        return null;
    }
}

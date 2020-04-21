﻿using System.Collections;
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

    public static EnhancedList<AbilityNodeHolder> globalList = new EnhancedList<AbilityNodeHolder>();

    // Given node ID.
    private int nodeId;
    private int centralThreadId;
    private int nodeThreadId;
    private Tuple<int, int> reference;
    private SpawnerOutput sourceObject;

    public Tuple<int, int> GetReference() {
        return reference;
    }

    public virtual void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

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

    public int GetCentralId() {
        return centralThreadId;
    }

    public void SetCentralId(int id) {
        centralThreadId = id;
    }

    public SpawnerOutput GetSourceObject() {
        return sourceObject;
    }

    public void SetSourceObject(SpawnerOutput srcObject) {
        sourceObject = srcObject;
    }

    public virtual void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("This Node", null)));
    }

    public virtual void NodeCallback(int threadId) {
        AbilityTreeNode refNode = GetNodeVariable<AbilityTreeNode>("This Node", VariableSetMode.LOCAL);

        if(refNode != null) {

            // Closes this game object as it is just a instance of another object.
            gameObject.SetActive(false);

            Debug.Log(refNode);

            Tuple<int, int> id = Tuple.Create<int, int>(centralThreadId, nodeId);

            // Removes previous instance.
            if(reference != null)
                GetInstanceCentralInst().RemoveSharedInstance(reference.Item2, id);

            // Adds current reference and creates a new instance according to reference.
            reference = refNode.reference;
            GetInstanceCentralInst().AddSharedInstance(reference.Item2, id);

        } else if(reference == null)
            reference = Tuple.Create<int, int>(centralThreadId, nodeId);

        //Debug.Log(CheckIfVarRegionBlocked("This Node") + " " + GetType());
        //Debug.Log(CheckIfVarRegionBlocked("Health") + " " + GetType());

        if(CheckIfVarRegionBlocked("This Node")) {
            SetVariable<AbilityTreeNode>("This Node", this,VariableSetMode.LOCAL);
            GetCentralInst().UpdateVariableValue<AbilityTreeNode>(nodeId, 0, null, false);
        }
    }

    public virtual void ThreadEndStartCallback(int threadId) {

    }

    public bool CheckIfVarRegionBlocked(params string[] target) {
        bool[] nodeBoolValues = AbilityCentralThreadPool.globalCentralList.l[centralThreadId].GetNodeBoolValues(nodeId);

        for(int i = 0; i < target.Length; i++)
            if(nodeBoolValues[GetVariableId(target[i])]) {
                Debug.LogWarning(target[i] + " is returning false.");
                return false;
            }

        return true;
    }

    public AbilityCentralThreadPool GetCentralInst() {
        return AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
    }

    public AbilityCentralThreadPool GetInstanceCentralInst() {
        return AbilityCentralThreadPool.globalCentralList.l[reference.Item1];
    }

    public bool IsClientPlayerUpdate() {
        return GetCentralInst().GetPlayerId() == ClientProgram.clientId;
    }


    // Function used by internal variable getter/setters to get central instance.
    AbilityCentralThreadPool InternalCentralReturn(VariableSetMode setMode) {
        switch(setMode) {

            case VariableSetMode.LOCAL:
                return GetCentralInst();

            case VariableSetMode.INSTANCE:
                return GetInstanceCentralInst();
        }

        return null;
    }

    public T GetNodeVariable<T>(string var, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        return InternalCentralReturn(setMode).ReturnRuntimeParameter<T>(GetNodeId(), var).v;
    }

    public void SetVariable<T>(int varId, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        InternalCentralReturn(setMode).UpdateVariableValue(nodeId, varId, value);
        InternalCentralReturn(setMode).NodeVariableCallback<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(int threadId, string varName, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        int varId = GetVariableId(varName);
        InternalCentralReturn(setMode).UpdateVariableValue(nodeId, varId, value);
        InternalCentralReturn(setMode).NodeVariableCallback<T>(threadId, varId);
    }

    public void SetVariable<T>(string varName, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        int varId = GetVariableId(varName);
        InternalCentralReturn(setMode).UpdateVariableValue(nodeId, varId, value);
        InternalCentralReturn(setMode).NodeVariableCallback<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(string varName, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        InternalCentralReturn(setMode).NodeVariableCallback<T>(nodeThreadId, GetVariableId(varName));
    }

    public int GetVariableId(string varName) {
        return LoadedData.loadedParamInstances[GetType()].variableAddresses[varName];
    }

    public virtual SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
        return null;
    }
}

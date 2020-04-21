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
        AbilityTreeNode refNode = GetNodeVariable<AbilityTreeNode>("This Node");

        if(refNode != null) {
            Tuple<int, int> id = Tuple.Create<int, int>(centralThreadId, nodeId);

            // Removes previous instance.
            GetCentralInst(VariableSetMode.INSTANCE).RemoveSharedInstance(reference.Item2, id);

            // Adds current reference and creates a new instance according to reference.
            reference = refNode.reference;
            GetCentralInst(VariableSetMode.INSTANCE).AddSharedInstance(reference.Item2, id);

        } else
            if(reference == null)
            reference = Tuple.Create<int, int>(centralThreadId, nodeId);

        if(CheckIfVarRegionBlocked("This Node"))
            SetVariable<AbilityTreeNode>("This Node", this);
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

    public AbilityCentralThreadPool GetCentralInst(VariableSetMode setMode  ) {
        switch(setMode) {

            case VariableSetMode.LOCAL:
                return AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

            case VariableSetMode.INSTANCE:
                return AbilityCentralThreadPool.globalCentralList.l[reference.Item1];
        }

        return null;
    }

    public T GetNodeVariable<T>(string var) {
        return GetCentralInst(VariableSetMode.INSTANCE).ReturnRuntimeParameter<T>(GetNodeId(), var).v;
    }

    public bool IsClientPlayerUpdate() {
        return GetCentralInst(VariableSetMode.LOCAL).GetPlayerId() == ClientProgram.clientId;
    }

    public void SetVariable<T>(int varId, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        GetCentralInst(setMode).UpdateVariableValue(nodeId, varId, value);
        GetCentralInst(setMode).NodeVariableCallback<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(int threadId, string varName, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        int varId = GetVariableId(varName);
        GetCentralInst(setMode).UpdateVariableValue(nodeId, varId, value);
        GetCentralInst(setMode).NodeVariableCallback<T>(threadId, varId);
    }

    public void SetVariable<T>(string varName, T value, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        int varId = GetVariableId(varName);
        GetCentralInst(setMode).UpdateVariableValue(nodeId, varId, value);
        GetCentralInst(setMode).NodeVariableCallback<T>(nodeThreadId, varId);
    }

    public void SetVariable<T>(string varName, VariableSetMode setMode = VariableSetMode.INSTANCE) {
        GetCentralInst(setMode).NodeVariableCallback<T>(nodeThreadId, GetVariableId(varName));
    }

    public int GetVariableId(string varName) {
        return LoadedData.loadedParamInstances[GetType()].variableAddresses[varName];
    }

    public virtual SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
        return null;
    }
}

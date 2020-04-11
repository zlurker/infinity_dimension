using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LinkMode {
    NORMAL, SIGNAL
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

    private SpawnerOutput sourceObject;

    public virtual void LinkEdit(int id,LinkData[] linkData,LinkModifier lM, Variable[][] var) {

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

    }

    public virtual void NodeCallback(int threadId) {

    }

    public virtual void ThreadEndStartCallback(int threadId) {

    }

    public bool CheckIfVarRegionBlocked(params int[] target) {
        bool[] nodeBoolValues = AbilityCentralThreadPool.globalCentralList.l[centralThreadId].GetNodeBoolValues(nodeId);

        for(int i = 0; i < target.Length; i++)
            if(nodeBoolValues[target[i]])
                return false;

        return true;
    }

    public AbilityCentralThreadPool GetCentralInst() {
        return AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
    }

    public T GetNodeVariable<T>(string var) {
        return GetCentralInst().ReturnRuntimeParameter<T>(GetNodeId(), var).v;
    }

    public bool IsClientPlayerUpdate() {
        return GetCentralInst().GetPlayerId() == ClientProgram.clientId;
    }
}

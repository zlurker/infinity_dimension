using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<AbilityTreeNode[]> globalList = new EnhancedList<AbilityTreeNode[]>();

    // Given node ID.
    private int nodeId;
    private int centralThreadId;
    private int nodeThreadId;

    private SpawnerOutput sourceObject;

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

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    // Used to specify if this class variables. If nothing, ALL is assumed to be default.
    public virtual VariableTypes[] ReturnVariableTypes() {
        return null;
    }

    public virtual void NodeCallback(int threadId) {
        //Debug.LogFormat("curr node {0}, nodeValue{1}, nodeThreadId{2}", nodeId, AbilityCentralThreadPool.globalCentralList.l[centralThreadId].ReturnRuntimeParameter<int>(nodeId, 0).v, nodeThreadId);
        //AbilityCentralThreadPool.globalCentralList.l[centralThreadId].NodeVariableCallback<int>(nodeThreadId, 0, AbilityCentralThreadPool.globalCentralList.l[centralThreadId].ReturnRuntimeParameter<int>(nodeId, 0).v);
    }

    public virtual void ThreadEndStartCallback(int threadId) {

    }

    public bool CheckIfVarRegionBlocked(int[] target) {
        bool[] nodeBoolValues = AbilityCentralThreadPool.globalCentralList.l[centralThreadId].GetNodeBoolValues(nodeId);

        for(int i = 0; i < target.Length; i++)
            if(nodeBoolValues[target[i]])
                return false;

        return true;
    }

    public void SyncDataWithNetwork<T>(int variableId, T value) {
        AbilityCentralThreadPool central = AbilityCentralThreadPool.globalCentralList.l[variableId];
        int centralId = central.ReturnNetworkObjectId();
        int centralInstId = central.ReturnInstId();

        UpdateAbilityDataEncoder inst = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.UPDATE_ABILITY_DATA] as UpdateAbilityDataEncoder;
        inst.SendUpdatedNodeData(centralId,centralInstId, nodeId, variableId, value);
    }

    public AbilityCentralThreadPool GetCentralInst() {
        return AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
    }

    public T GetNodeVariable<T>(int varId) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        return GetCentralInst().ReturnRuntimeParameter<T>(GetNodeId(), varId).v;
    }
}

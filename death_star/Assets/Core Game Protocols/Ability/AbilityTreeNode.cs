using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType {
    DEFAULT, GETEND, GET
}

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<ScriptableObject[]> globalList = new EnhancedList<ScriptableObject[]>();

    // Given node ID.
    int nodeId;
    int centralThreadId;
    int nodeThreadId;

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


    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public virtual void NodeCallback(int nId, int variableCalled, VariableAction action) {

        Debug.LogFormat("prev nodeId {0}, curr node {1}, nodeValue{2}", nId, nodeId, TravelThread.globalCentralList.l[centralThreadId].ReturnVariable<string>(nodeId, 0).v);
        TravelThread.globalCentralList.l[centralThreadId].NodeVariableCallback<string>(nodeThreadId, 0, TravelThread.globalCentralList.l[centralThreadId].ReturnVariable<string>(nodeId, 0).v);
    }
}

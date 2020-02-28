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

    public int GetCentralId() {
        return centralThreadId;
    }

    public void SetCentralId(int id) {
        centralThreadId = id;
    }

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public virtual void NodeCallback(int threadId) {
        Debug.LogFormat("curr node {0}, nodeValue{1}, nodeThreadId{2}", nodeId, AbilityCentralThreadPool.globalCentralList.l[centralThreadId].ReturnRuntimeParameter<int>(nodeId, 0).v,nodeThreadId);
        AbilityCentralThreadPool.globalCentralList.l[centralThreadId].NodeVariableCallback<int>(nodeThreadId, 0, AbilityCentralThreadPool.globalCentralList.l[centralThreadId].ReturnRuntimeParameter<int>(nodeId, 0).v);
    }

    /*public virtual void OnLoopThreadBegin(int threadId) {

    }*/

    public virtual void ThreadEndStartCallback(int threadId) {

    }
}

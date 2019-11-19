using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<AbilityTreeNode[]> globalList = new EnhancedList<AbilityTreeNode[]>();

    // Given node ID.
    int nodeId;

    // Link to tree transverser.
    int treeTransverser;

    // Variables in node.
    Variable[] runtimeParameters;

    int currFireCount;

    public void RunNodeInitialisation(Variable[] rP, int nid, int tt) {
        runtimeParameters = rP;
        nodeId = nid;
        treeTransverser = tt;
    }

    public Variable[] GetVariables() {
        return runtimeParameters;
    }

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public void FireNode(int variable, VariableAction action) {
        GetTransverser().TransversePoint(nodeId, variable, action);
    }

    public virtual bool NodeCallback(int nId, int variableCalled, VariableAction action) {
        bool firstCallback = false;

        if(!gameObject.activeSelf) {
            gameObject.SetActive(true);
            firstCallback = true;
        }
       
        Debug.Log("nodeId " + nodeId + "\ncalled by : " + nId);
        return firstCallback;
    }

    public TreeTransverser GetTransverser() {
        return TreeTransverser.globalList.l[treeTransverser];
    }
}

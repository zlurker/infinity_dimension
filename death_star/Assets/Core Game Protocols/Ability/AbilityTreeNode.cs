using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<AbilityTreeNode[]> globalList = new EnhancedList<AbilityTreeNode[]>();

    // Given node ID.
    int nodeId;

    // Link to tree transverser.
    int treeTransverser;

    int currFireCount;

    public void RunNodeInitialisation(int nid, int tt) {
        //runtimeParameters = rP;
        nodeId = nid;
        treeTransverser = tt;
    }

    /*public Variable[] GetVariables() {
        //return runtimeParameters;
    }*/

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public void FireNode(int variable, VariableAction action) {
        GetTransverser().TransversePoint(nodeId, variable, action);
    }

    public virtual void NodeCallback(int nId, int variableCalled, VariableAction action) {       
        Debug.Log("nodeId " + nodeId + "\ncalled by : " + nId);
    }

    public TreeTransverser GetTransverser() {
        return TreeTransverser.globalList.l[treeTransverser];
    }
}

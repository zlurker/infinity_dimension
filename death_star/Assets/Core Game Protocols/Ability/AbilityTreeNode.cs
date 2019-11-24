using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<AbilityTreeNode[]> globalList = new EnhancedList<AbilityTreeNode[]>();

    // Given node ID.
    int nodeId;

    // Link to current parent tree transverser.
    int treeTransverser;

    // Link to root tree transverser.
    int rootTreeTransverser;

    public int GetNodeId() {
        return nodeId;
    }

    public void SetRootTransverer(int id) {
        rootTreeTransverser = id;
    }

    public int GetRootTransverser() {
        return rootTreeTransverser;
    }

    public void SetParentTransverser(int id) {
        treeTransverser = id;
    }

    public virtual void RunNodeInitialisation(int nid, int tt,int rtt) {
        //runtimeParameters = rP;
        nodeId = nid;
        treeTransverser = tt;
        rootTreeTransverser = rtt;
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

        if(treeTransverser < 0 || treeTransverser >= TreeTransverser.globalListTree.l.Count)
            return null;

        return TreeTransverser.globalListTree.l[treeTransverser];
    }

    // Call this on node tasking finish.
    public void NodeTaskingFinish() {
        GetTransverser().NodeTaskingFinished();
    }
}

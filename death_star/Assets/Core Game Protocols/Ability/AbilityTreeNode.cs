using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<ScriptableObject[]> globalList = new EnhancedList<ScriptableObject[]>();

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

    public virtual void RunNodeInitialisation(int nid, int tt, int rtt) {
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
        GetTransverserObject().TransversePoint(nodeId, variable, action);
    }

    public virtual void NodeCallback(int nId, int variableCalled, VariableAction action) {
        Debug.Log("nodeId " + nodeId + "\ncalled by : " + nId);
    }

    public int GetTransverser() {
        return treeTransverser;
    }

    public TreeTransverser GetTransverserObject() {
        //Debug.Log(treeTransverser);
        //Debug.Log(treeTransverser < 0);
        //Debug.Log(treeTransverser >= TreeTransverser.globalListTree.l.Count);

        if(treeTransverser < 0 || treeTransverser >= TreeTransverser.globalListTree.l.Count)
            return null;

        //Debug.Log(TreeTransverser.globalListTree.l[treeTransverser]);
        return TreeTransverser.globalListTree.l[treeTransverser];
    }

    // Call this on node tasking finish.
    public void NodeTaskingFinish() {
        GetTransverserObject().NodeTaskingFinished(nodeId);
    }

    public virtual void ClearObject() {
        //Debug.Log("For extra functions when this is required.");
    }
}

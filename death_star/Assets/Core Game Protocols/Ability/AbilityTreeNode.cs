﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType {
    DEFAULT, GETEND
}

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<ScriptableObject[]> globalList = new EnhancedList<ScriptableObject[]>();

    // Given node ID.
    int nodeId;
    int centralThreadId;
    int nodeThreadId;

    // ------------Beyond this point variables are considered.--------------

    // Link to current parent tree transverser.
    int treeTransverser;

    // Link to root tree transverser.
    int rootTreeTransverser;

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

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public virtual void NodeCallback(int nId, int variableCalled, VariableAction action) {
        Debug.Log("nodeId " + nodeId + "\ncalled by : " + nId);
        //Debug.Log((GetTransverserObject().GetVariable(nId)[variableCalled].field as RuntimeParameters<string>).v);
    }

    public int GetTransverser() {
        return treeTransverser;
    }

    /*public T GetVariableValue<T>(int node,int variableId) {
        return (TreeTransverser.globalListTree.l[rootTreeTransverser].GetVariable(node)[variableId].field as RuntimeParameters<T>).v;
    }

    public void SetVariableValue<T>(int variableId, T value) {
        (TreeTransverser.globalListTree.l[rootTreeTransverser].GetVariable(nodeId)[variableId].field as RuntimeParameters<T>).v = value;//GetRuntimeParameters()[variableId] as RuntimeParameters<T>).v = value;
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
        if(GetTransverserObject() != null)
            GetTransverserObject().NodeTaskingFinished(nodeId);
    }

    public AbilityTreeNode GetNodeFromScriptable(ScriptableObject inst) {
        return Spawner.GetCType<AbilityTreeNode>(inst);
    }*/
}

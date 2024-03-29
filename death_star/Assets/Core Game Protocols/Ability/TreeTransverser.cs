﻿/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeTransverser : AbilityTreeNode {

    public static EnhancedList<TreeTransverser> globalListTree = new EnhancedList<TreeTransverser>();

    //Variables here are used by both main and sub.
    int transverserId;
    bool treeTransverseCompleted;

    // Iteration count used by treetransverser to track cycle.
    int givenIterationCount = 1;
    int currIterationCount = 0;

    // Variables below are carried by main transversers.
    // Variables in node.
    Variable[][] runtimeParameters;   
    Type[] subclassTypes;

    // Used to automate when a subclass has completed its functions.
    bool[][] completion;
    int[] subclassCompletion;

    // Link to ability nodes.
    int abilityNodes;

    int branchCount;
    int[] branchEndData;
    int[] branchStartData;
    int[] nodeType;
    int defaultId;

    public void ResetTransverser() {
        treeTransverseCompleted = false;
        currIterationCount = 0;
        SetUpCompletion();
    }

    public void SetUpCompletion() {
        completion = new bool[runtimeParameters.Length][];
        subclassCompletion = new int[runtimeParameters.Length];

        for (int i =0; i < runtimeParameters.Length; i++) {
            completion[i] = new bool[runtimeParameters[i].Length];
            subclassCompletion[i] = runtimeParameters[i].Length;
        }
    }

    public Variable[] GetVariable(int id) {
        return runtimeParameters[id];
    }

    public int GetVariableCount(int id) {
        return runtimeParameters[id].Length;
    }

    public override void RunNodeInitialisation(int nid, int tt, int rtt) {
        base.RunNodeInitialisation(nid, tt, rtt);
        transverserId = globalListTree.Add(this);
        branchCount = GetRootTransverserObject().branchEndData[GetNodeId()];
        ResetTransverser();
    }

    public void SetTransverserId(int id) {
        transverserId = id;
    }

    public void SetRootTransverserData(Variable[][] rP, Type[] t, int rtt, int dId) {
        runtimeParameters = rP;
        subclassTypes = t;

        SetRootTransverer(rtt);
        SetParentTransverser(-1);
        SetNodeId(-1);

        defaultId = dId;
        ResetTransverser();
    }

    public void SetNodeData(int id, int[] eD, int[] sD, int[] nT) {
        abilityNodes = id;
        branchEndData = eD;
        branchStartData = sD;
        nodeType = nT;
    }

    public void TransversePoint(int nodeId, int variableId, VariableAction action) {

        int[][] nextNodeIdArray = GetRootTransverserObject().runtimeParameters[nodeId][variableId].links[(int)action];

        //Debug.LogFormat("Curr node: {0}, Curr pathCount: {1}, id {2}", nodeId, branchCount, transverserId);
        //Debug.Log("Next trnse:" + nextNodeIdArray.Length);

        for(int i = 0; i < nextNodeIdArray.Length; i++) {
            CreateNewNodeIfNull(nextNodeIdArray[i][0]);

            switch((NodeType)GetRootTransverserObject().nodeType[nodeId]) {
                case NodeType.DEFAULT:
                    RunAllGetInNode(nextNodeIdArray[i][0]);
                    break;

                case NodeType.GETEND:
                    Debug.Log("It is a getend, next node: " + nextNodeIdArray[i][0]);
                    break;
            }

           
            GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][nextNodeIdArray[i][0]]).NodeCallback(nodeId, variableId, action);           
        }
    }

    public void RunAllGetInNode(int nodeId) {
        int varCount = globalListTree.l[GetRootTransverser()].GetVariableCount(nodeId);

        for(int i = 0; i < varCount; i++) 
            GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][nodeId]).FireNode(i, VariableAction.GET);        
    }

    public void AddBranches(int nodeId) {
        if(GetRootTransverserObject().branchEndData[nodeId] > 1)
            branchCount += GetRootTransverserObject().branchEndData[nodeId] - 1;
    }

    public void RemoveBranches(int nodeId) {
        if(GetRootTransverserObject().branchEndData[nodeId] == 0)
            branchCount--;
    }


    public void CreateNewNodeIfNull(int nodeId) {
        if(!globalList.l[GetRootTransverserObject().abilityNodes][nodeId]) {
            globalList.l[GetRootTransverserObject().abilityNodes][nodeId] = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { GetRootTransverserObject().subclassTypes[nodeId] });
            GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][nodeId]).RunNodeInitialisation(nodeId, transverserId, GetRootTransverser());
        }

        AddBranches(nodeId);
    }

    public void NodeTaskingFinished(int nodeId) {

        RemoveBranches(nodeId);

        if(!treeTransverseCompleted)
            if(branchCount == 0) {
                treeTransverseCompleted = true;
                Debug.LogFormat("We have reached the end of the path, id {0}.", transverserId);

                if(BeginNodeCallback())
                    return;

                if(GetTransverser() > -1) {
                    // Debug.LogFormat("Task Finished Called {0}", transverserId);
                    GetTransverserObject().branchCount -= GetRootTransverserObject().branchEndData[GetNodeId()];
                    NodeTaskingFinish();

                } else {

                    for(int i = 0; i < AbilityTreeNode.globalList.l[abilityNodes].Length; i++) {
                        //GetNodeFromScriptable(globalList.l[abilityNodes][i]).ClearObject();
                        //Singleton.GetSingleton<Spawner>().Remove(AbilityTreeNode.globalList.l[abilityNodes][i]);
                    }

                    //globalList.Remove(abilityNodes);
                    //globalListTree.Remove(transverserId);
                    //AbilitiesManager.RemoveExpiredTree(defaultId);
                }
            }
    }

    public bool BeginNodeCallback() {

        bool below = false;

        if(currIterationCount < givenIterationCount) {

            below = true;
            currIterationCount++;

            if(GetNodeId() > -1) {
                // Node callback for elements in a node group.
                Variable[] nodeVariables = GetRootTransverserObject().runtimeParameters[GetNodeId()];

                for(int i = 0; i < nodeVariables.Length; i++)
                    TransversePoint(GetNodeId(), i, VariableAction.SET);

            } else {
                // Callback for root.
                branchCount = branchStartData.Length;
                for(int i = 0; i < branchStartData.Length; i++) {
                    CreateNewNodeIfNull(GetRootTransverserObject().branchStartData[i]);
                    RunAllGetInNode(branchStartData[i]);
                    //GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][GetRootTransverserObject().branchStartData[i]]).NodeCallback(GetRootTransverserObject().branchStartData[i], 0, 0);            
                }
            }
        }

        return below;
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        BeginNodeCallback();
    }

    public TreeTransverser GetRootTransverserObject() {
        return globalListTree.l[GetRootTransverser()];
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Iterator Times",1)
        };
    }

    public override void ClearObject() {
        //base.ClearObject();
        globalListTree.Remove(transverserId);
    }
}
*/
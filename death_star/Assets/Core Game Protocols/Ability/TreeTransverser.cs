using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeTransverser {

    public static EnhancedList<TreeTransverser> globalList = new EnhancedList<TreeTransverser>();

    int transverserId;

    // Variables in node.
    Variable[][] runtimeParameters;
    Type[] subclassTypes;

    // Link to ability nodes.
    int abilityNodes;

    int branchCount;

    int[] branchEndData;

    int[] branchStartData;

    public TreeTransverser() {

        // Sets default max transvering.
        branchCount = 0;
    }

    public void SetTransverserId(int id) {
        transverserId = id;
    }

    public void SetVariableNetworkData(Variable[][] rP, Type[] t) {
        runtimeParameters = rP;
        subclassTypes = t;
    }

    public void SetNodeData(int id, int[] eD, int[] sD, int root) {
        abilityNodes = id;
        branchEndData = eD;
        branchStartData = sD;
        branchCount = root;
    }

    public void StartTreeTransverse() {
        for(int i = 0; i < branchStartData.Length; i++)
            for(int j = 0; j < runtimeParameters[branchStartData[i]].Length; j++) {
                //defaultTransverser.TransversePoint(rootSubclasses[i], j, (VariableAction)0);
                CreateNewNodeIfNull(branchStartData[i]);
                TransversePoint(branchStartData[i], j, (VariableAction)1);
            }
    }

    public void TransversePoint(int nodeId, int variableId, VariableAction action) {
        //runtimeParameters[nodeId][variableId].links
        int[][] nextNodeIdArray = runtimeParameters[nodeId][variableId].links[(int)action];

        Debug.LogFormat("Curr node: {0}, Curr pathCount: {1}", ((RuntimeParameters<string>)runtimeParameters[nodeId][variableId].field).v, branchCount);

        for(int i = 0; i < nextNodeIdArray.Length; i++) {
            CreateNewNodeIfNull(nextNodeIdArray[i][0]);
            AbilityTreeNode.globalList.l[abilityNodes][nextNodeIdArray[i][0]].NodeCallback(nodeId, variableId, action);
        }
    }

    public void DoBranchCalculation(int nodeId) {
        if(branchEndData[nodeId] > 1) 
            branchCount += branchEndData[nodeId] - 1;
        
        if(branchEndData[nodeId] == 0)
            branchCount--;

        if(branchCount == 0)
            Debug.Log("We have reached the end of the path.");
    }

    public void CreateNewNodeIfNull(int nodeId) {
        if(!AbilityTreeNode.globalList.l[abilityNodes][nodeId]) {
            AbilityTreeNode.globalList.l[abilityNodes][nodeId] = Spawner.GetCType(Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { subclassTypes[nodeId] }), subclassTypes[nodeId]) as AbilityTreeNode;
            AbilityTreeNode.globalList.l[abilityNodes][nodeId].RunNodeInitialisation(nodeId, transverserId);           
        }

        DoBranchCalculation(nodeId);
    }
}

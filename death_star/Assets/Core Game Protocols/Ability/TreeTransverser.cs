using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeTransverser : AbilityTreeNode {

    public static EnhancedList<TreeTransverser> globalListTree = new EnhancedList<TreeTransverser>();

    int transverserId;

    // Variables in node.
    Variable[][] runtimeParameters;
    Type[] subclassTypes;

    // Link to ability nodes.
    int abilityNodes;

    int branchCount;

    int[] branchEndData;
    int[] branchStartData;

    public override void RunNodeInitialisation(int nid, int tt, int rtt) {
        base.RunNodeInitialisation(nid, tt, rtt);
        transverserId = globalListTree.Add(this);
        branchCount = GetRootTransverserObject().branchEndData[GetNodeId()];
    }

    public void SetTransverserId(int id) {
        transverserId = id;
    }

    public void SetVariableNetworkData(Variable[][] rP, Type[] t, int rtt) {
        runtimeParameters = rP;
        subclassTypes = t;
        SetRootTransverer(rtt);
        SetParentTransverser(-1);
    }

    public void SetNodeData(int id, int[] eD, int[] sD, int root) {
        abilityNodes = id;
        branchEndData = eD;
        branchStartData = sD;
        branchCount = root;
    }

    public void StartTreeTransverse() {
        for(int i = 0; i < branchStartData.Length; i++)
            for(int j = 0; j < GetRootTransverserObject().runtimeParameters[GetRootTransverserObject().branchStartData[i]].Length; j++) {
                //defaultTransverser.TransversePoint(rootSubclasses[i], j, (VariableAction)0);
                CreateNewNodeIfNull(GetRootTransverserObject().branchStartData[i]);
                GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][GetRootTransverserObject().branchStartData[i]]).NodeCallback(GetRootTransverserObject().branchStartData[i], 0, 0);
                // To be changed.

                //TransversePoint(GetRootTransverserObject().branchStartData[i], j, (VariableAction)1);
            }
    }

    public void TransversePoint(int nodeId, int variableId, VariableAction action) {
        //runtimeParameters[nodeId][variableId].links
        int[][] nextNodeIdArray = GetRootTransverserObject().runtimeParameters[nodeId][variableId].links[(int)action];

        Debug.LogFormat("Curr node: {0}, Curr pathCount: {1}, id {2}", ((RuntimeParameters<string>)GetRootTransverserObject().runtimeParameters[nodeId][variableId].field).v, branchCount, transverserId);

        for(int i = 0; i < nextNodeIdArray.Length; i++) {
            CreateNewNodeIfNull(nextNodeIdArray[i][0]);
            GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][nextNodeIdArray[i][0]]).NodeCallback(nodeId, variableId, action);
        }
    }

    public void DoBranchCalculation(int nodeId) {
        if(GetRootTransverserObject().branchEndData[nodeId] > 1)
            branchCount += GetRootTransverserObject().branchEndData[nodeId] - 1;

        if(GetRootTransverserObject().branchEndData[nodeId] == 0)
            branchCount--;
    }

    public void CreateNewNodeIfNull(int nodeId) {
        if(!AbilityTreeNode.globalList.l[GetRootTransverserObject().abilityNodes][nodeId]) {
            AbilityTreeNode.globalList.l[GetRootTransverserObject().abilityNodes][nodeId] = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { GetRootTransverserObject().subclassTypes[nodeId] });
            GetNodeFromScriptable(globalList.l[GetRootTransverserObject().abilityNodes][nodeId]).RunNodeInitialisation(nodeId, transverserId, GetRootTransverser());
        }

        DoBranchCalculation(nodeId);
    }

    public AbilityTreeNode GetNodeFromScriptable(ScriptableObject inst) {
        return Spawner.GetCType<AbilityTreeNode>(inst);
    }

    // Method will be called on every node tasking end.
    public void NodeTaskingFinished() {

        if(branchCount == 0) {
            Debug.LogFormat("We have reached the end of the path, id {0}.",transverserId);

            TreeTransverser parent = GetTransverserObject();

            if(GetTransverser() > -1) {
                Debug.LogFormat("Task Finished Called {0}", transverserId);
                parent.branchCount -= GetRootTransverserObject().branchEndData[GetNodeId()];
                NodeTaskingFinish();               
            } else {
                for(int i = 0; i < AbilityTreeNode.globalList.l[abilityNodes].Length; i++) {
                    GetNodeFromScriptable(globalList.l[abilityNodes][i]).ClearObject();
                    Singleton.GetSingleton<Spawner>().Remove(AbilityTreeNode.globalList.l[abilityNodes][i]);
                }
                
                globalList.Remove(abilityNodes);
            }
        }
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        //base.NodeCallback(nId, variableCalled, action);

        Variable[] nodeVariables = GetRootTransverserObject().runtimeParameters[GetNodeId()];

        for(int i = 0; i < nodeVariables.Length; i++) 
            TransversePoint(GetNodeId(), i, VariableAction.SET);       
    }

    public TreeTransverser GetRootTransverserObject() {
        return globalListTree.l[GetRootTransverser()];
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Test","V20")
        };
    }
}

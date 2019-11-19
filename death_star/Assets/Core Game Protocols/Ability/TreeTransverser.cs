using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTransverser {

    public static EnhancedList<TreeTransverser> globalList = new EnhancedList<TreeTransverser>();

    // Link to ability nodes.
    int abilityNodes;

    int branchCount;

    int[] branchEndData;

    public TreeTransverser() {

        // Sets default max transvering.
        branchCount = 0;
    }

    public void SetNodeData(int id, int[] eD,int root) {
        abilityNodes = id;
        branchEndData = eD;
        branchCount = root;
    }

    public void TransversePoint(int nodeId, int variableId, VariableAction action) {
        int[][] nextNodeIdArray = AbilityTreeNode.globalList.l[abilityNodes][nodeId].GetVariables()[variableId].links[(int)action];

        if(branchEndData[nodeId] == 0)
            branchCount--;

        Debug.LogFormat("Curr node: {0}, Curr pathCount: {1}", nodeId,branchCount);

        if(branchCount == 0)
            Debug.Log("We have reached the end of the path.");

        for(int i = 0; i < nextNodeIdArray.Length; i++) {            
            bool result = AbilityTreeNode.globalList.l[abilityNodes][nextNodeIdArray[i][0]].NodeCallback(nodeId, variableId, action);

            if(result)
                if(branchEndData[nextNodeIdArray[i][0]] > 1)
                    branchCount += branchEndData[abilityNodes];
        }      
    }
}

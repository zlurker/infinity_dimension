using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTransverser {

    public static EnhancedList<TreeTransverser> globalList = new EnhancedList<TreeTransverser>();

    // Link to ability nodes.
    public int abilityNodes;

    // Max trasversing of a point in tree allowed.
    public int maxTransverse;

    public TreeTransverser() {

        // Sets default max transvering.
        maxTransverse = 1;
    }

    public void TransversePoint(int nodeId, int variableId, VariableAction action) {
        int[][] nextNodeIdArray = AbilityTreeNode.globalList.l[abilityNodes][nodeId].runtimeParameters[variableId].links[(int)action];
        
        for (int i= 0; i < nextNodeIdArray.Length; i++) {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    public static EnhancedList<AbilityTreeNode[]> globalList = new EnhancedList<AbilityTreeNode[]>();

    // Given node ID.
    int nodeId;

    // Link to tree transverser.
    public int treeTransverser;

    // Variables in node.
    public Variable[] runtimeParameters;

    // Counts in tranversing for outgoing notes on get and set.
    public int[,] transverseCount;

    public void RunNodeInitialisation(Variable[] rP) {
        runtimeParameters = rP;
        transverseCount = new int[runtimeParameters.Length, 2];
    }

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }

    public virtual void NodeCallback(int nodeId, VariableAction action) {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);

        Debug.Log("called by: " + nodeId);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTreeNode : MonoBehaviour {

    // Ability ID.

    // Given node ID.

    // Link to tree transverser.


    // Variables in node.
    Variable[] runtimeParameters;

    // Find out which stage is outgoing node variable at.
    int[] tranverseId;

    public virtual RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[0];
    }
}

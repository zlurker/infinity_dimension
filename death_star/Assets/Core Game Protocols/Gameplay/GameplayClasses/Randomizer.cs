using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : AbilityTreeNode {

    public static int LOWEST_RANGE = 0;
    public static int HIGHEST_RANGE = 1;

    int nodeId;
    // Use this for initialization
    void Start() {

    }

    void Update() {
        
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<GameObject>("Input Object", null)
        };
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        nodeId = nId;
        //GetVariableValue<GameObject>(nodeId, 0).transform.position += new Vector3(1, 1);
        NodeTaskingFinish();
    }
}

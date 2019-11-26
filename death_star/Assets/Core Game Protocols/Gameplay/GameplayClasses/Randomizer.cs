using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : AbilityTreeNode {

    public static int LOWEST_RANGE =0;
    public static int HIGHEST_RANGE = 1;

	// Use this for initialization
	void Start () {
		
	}
	
]	void Update () {
		
	}

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float>("Lowest Range", -10),
            new RuntimeParameters<float>("Highest Range", 10)
        };
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        FireNode(0, VariableAction.GET);
        FireNode(1, VariableAction.GET);

        Random.Range(GetVariableValue<float>(LOWEST_RANGE), GetVariableValue<float>(HIGHEST_RANGE));

        FireNode(0, VariableAction.SET);
        FireNode(1, VariableAction.SET);
        NodeTaskingFinish();
    }
}

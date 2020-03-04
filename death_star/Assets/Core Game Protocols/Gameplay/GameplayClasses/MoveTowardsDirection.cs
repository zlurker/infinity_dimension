using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsDirection : AbilityTreeNode {

    public const int DIRECTION_FROM_TARGET = 0;
    public const int TARGET = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float[]>("Direction From Target",null),
            new RuntimeParameters<Transform>("Target",null)
        };
    }
}

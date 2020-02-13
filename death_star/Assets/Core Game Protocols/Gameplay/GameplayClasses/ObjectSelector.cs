using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : AbilityTreeNode {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("PosX", 0),
            new RuntimeParameters<int>("PosY", 0)
        };
    }
}

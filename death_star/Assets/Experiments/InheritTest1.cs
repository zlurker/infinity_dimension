using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritTest1 : AbilityTreeNode {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<float>("1-1", 1)));
    }
}

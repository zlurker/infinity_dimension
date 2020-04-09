using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritTest2 : InheritTest1 {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("2-1", 2)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("2-2", 2))
        });
    }
}

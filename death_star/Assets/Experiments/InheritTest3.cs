using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritTest3 : InheritTest2 {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("3-1", 4)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("3-2", 5)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("3-3", 6))
        });
    }
}

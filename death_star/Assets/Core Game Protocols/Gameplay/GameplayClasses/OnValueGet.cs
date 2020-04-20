using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnValueGet : NodeModifierBase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Current Value",0),VariableTypes.INTERCHANGEABLE),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Modified Value To Return",0),VariableTypes.INTERCHANGEABLE)
        });
    }
}

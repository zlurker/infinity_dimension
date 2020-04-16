using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableInterfaces {

    public List<SharedVariable> sVList;
}

public class SharedVariable : AbilityTreeNode, IOnSpawn {

    public static Dictionary<int, VariableInterfaces> sharedVariables = new Dictionary<int, VariableInterfaces>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
                
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Variable Name","")),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Variable Value",0),VariableTypes.INTERCHANGEABLE)
        });
    }

    public void OnSpawn() {
        
    }
}

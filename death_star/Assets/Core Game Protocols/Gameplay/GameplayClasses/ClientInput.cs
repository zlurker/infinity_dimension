using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    bool inputSet;

    public void InputCallback(int callbackData) {
        inputSet = false;
        SetVariable<int>("Input Key");    
    }

    public override void NodeCallback() {
        base.NodeCallback();

        Debug.Log("Input working");

        if(IsClientPlayerUpdate()) {
            if(!inputSet) 
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>("Input Key"), 1);
            
            inputSet = true;
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0), VariableTypes.CLIENT_ACTIVATED,VariableTypes.SIGNAL_ONLY)
        });
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

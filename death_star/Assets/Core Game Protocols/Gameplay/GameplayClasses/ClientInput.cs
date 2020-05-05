using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    bool inputSet;

    private void Update() {
        if (GetNodeVariable<int>("Max") > GetNodeVariable<int>("Curr")) {
            
            SetVariable<int>("Curr", GetNodeVariable<int>("Curr") + 1);
            SetVariable<int>("Input Key");
            Debug.LogFormat("Curr: {0}, Max: {1}", GetNodeVariable<int>("Curr"), GetNodeVariable<int>("Max"));
        }
    }

    public void InputCallback(int callbackData) {
        inputSet = false;
        //Debug.Log("Input called");
        SetVariable<int>("Max", GetNodeVariable<int>("Max") + 1);
        //SetVariable<int>("Input Key");    
    }

    public override void NodeCallback() {
        base.NodeCallback();

        if(IsClientPlayerUpdate()) {
            if(!inputSet) 
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>("Input Key"), 1);
            
            inputSet = true;
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0), VariableTypes.SIGNAL_ONLY),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Curr", 0), VariableTypes.HIDDEN),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Max", 0), VariableTypes.CLIENT_ACTIVATED,VariableTypes.HIDDEN)
        });
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

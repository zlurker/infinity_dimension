using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    bool inputSet;

    public void InputCallback(int callbackData) {
        Debug.Log("Input callback, Client has been inputted.");
        inputSet = false;
        SetVariable<int>("Input Key");    
    }

    public override void NodeCallback(int threadId) {
        Debug.Log("Input working");

        if(IsClientPlayerUpdate()) {
            if(!inputSet) {
                Debug.Log(GetNodeVariable<int>("Input Key"));
                Debug.Log((KeyCode)GetNodeVariable<int>("Input Key"));
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>("Input Key"), 1);
            }

            inputSet = true;
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0), VariableTypes.CLIENT_ACTIVATED)
        });
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    public const int INPUT_KEY = 0;

    bool inputSet;

    public void InputCallback(int callbackData) {
        Debug.Log("Input callback, Client has been inputted.");
        inputSet = false;
        GetCentralInst().NodeVariableCallback<int>(GetNodeThreadId(),INPUT_KEY, 0);       
    }

    public override void NodeCallback(int threadId) {
        Debug.Log("Input working");

        if(IsClientPlayerUpdate()) {
            if(!inputSet) {
                Debug.Log(GetNodeVariable<int>(INPUT_KEY));
                Debug.Log((KeyCode)GetNodeVariable<int>(INPUT_KEY));
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>(INPUT_KEY), 1);
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

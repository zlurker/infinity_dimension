using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    public const int INPUT_KEY = 0;

    bool inputSet;

    public void InputCallback(int callbackData) {
        Debug.Log("Input callback, Client has been inputted.");
        GetCentralInst().NodeVariableCallback<int>(GetNodeThreadId(),INPUT_KEY, 0);
        inputSet = false;
    }

    public override void NodeCallback(int threadId) {
        Debug.Log("Input working");

        if(IsClientPlayerUpdate()) {
            if(!inputSet)
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>(INPUT_KEY), 1);

            inputSet = true;
        }
    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key",0),VariableTypes.SIGNAL_VAR,VariableTypes.CLIENT_ACTIVATED)
        };
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

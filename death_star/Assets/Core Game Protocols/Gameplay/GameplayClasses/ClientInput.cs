using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn {

    public const int INPUT_KEY = 0;

    bool inputSet;

    public void InputCallback(int callbackData) {
        Debug.Log("Input callback, Client has been inputted.");
        SyncDataWithNetwork<int>(INPUT_KEY, 0,VariableTypes.SIGNAL_VAR);
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

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Input Key",0)
        };
    }

    public void OnSpawn() {
        inputSet = false;
    }
}

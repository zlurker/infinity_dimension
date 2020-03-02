using System.Collections.Generic;
using UnityEngine;

public interface IInputCallback<T> {
    void InputCallback(T callbackData);
}

public class InputData<T> : InputDataBase {
    public IInputCallback<T> inputCallback;
    public T callbackParameter;

    public InputData(T cbParam, IInputCallback<T> iCB, KeyCode kC, int b, int iT) {
        callbackParameter = cbParam;
        inputCallback = iCB;

        keyCode = kC;
        button = b;
        inputType = iT;
    }

    public override void InputCallback() {
        inputCallback.InputCallback(callbackParameter);
    }

}

public class InputDataBase {

    public KeyCode keyCode;
    public int button;
    public int inputType;

    public virtual void InputCallback() {

    }
}

public class PlayerInput : MonoBehaviour, ISingleton {

    public List<InputDataBase> iS;

    void Update() {
        if(iS.Count > 0)
            for(int i = iS.Count - 1; i >= 0; i--) {

                bool inputHappened = false;

                // Test for Keyboard.
                if(iS[i].keyCode != KeyCode.None) {
                    switch(iS[i].inputType) {
                        case 0:
                            if(Input.GetKeyDown(iS[i].keyCode))
                                inputHappened = true;
                            break;

                        case 1:
                            if(Input.GetKey(iS[i].keyCode))
                                inputHappened = true;
                            break;

                        case 2:
                            if(Input.GetKeyUp(iS[i].keyCode))
                                inputHappened = true;
                            break;
                    }

                    // Test for mouse.
                } else {
                    switch(iS[i].inputType) {
                        case 0:
                            if(Input.GetMouseButtonDown(iS[i].button))
                                inputHappened = true;
                            break;

                        case 1:
                            if(Input.GetMouseButton(iS[i].button))
                                inputHappened = true;
                            break;

                        case 2:
                            if(Input.GetMouseButtonUp(iS[i].button))
                                inputHappened = true;
                            break;
                    }
                }

                if(inputHappened) {
                    iS[i].InputCallback();
                    iS.RemoveAt(i);
                }
            }
    }

    public void AddNewInput<T>(IInputCallback<T> iCB, T cbParam, KeyCode key, int b, int iT) {
        iS.Add(new InputData<T>(cbParam, iCB, key, b, iT));
    }

    public void RunOnStart() {
        iS = new List<InputDataBase>();
    }

    public void RunOnCreated() {
        iS = new List<InputDataBase>();
    }

}
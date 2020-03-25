using System.Collections.Generic;
using UnityEngine;

public interface IInputCallback<T> {
    void InputCallback(T callbackData);
}

public class InputData<T> : InputDataBase {
    public IInputCallback<T> inputCallback;
    public T callbackParameter;

    public InputData(T cbParam, IInputCallback<T> iCB, KeyCode kC, int iT, bool perm) {
        callbackParameter = cbParam;
        inputCallback = iCB;

        keyCode = kC;
        inputType = iT;
        permanent = perm;
    }

    public override void InputCallback() {
        inputCallback.InputCallback(callbackParameter);
    }

}

public class InputDataBase {

    public KeyCode keyCode;
    public int inputType;
    public bool permanent;

    public virtual void InputCallback() {

    }
}

public class PlayerInput : MonoBehaviour, ISingleton {

    public List<InputDataBase> iS;

    void Update() {

        if(iS.Count > 0)
            for(int i = iS.Count - 1; i >= 0; i--) {

                bool inputTriggered = false;

                switch(iS[i].inputType) {
                    case 0:
                        if(Input.GetKeyDown(iS[i].keyCode))
                            inputTriggered = true;
                        break;

                    case 1:
                        if(Input.GetKey(iS[i].keyCode))
                            inputTriggered = true;
                        break;

                    case 2:
                        if(Input.GetKeyUp(iS[i].keyCode))
                            inputTriggered = true;
                        break;
                }

                if(inputTriggered) {
                    iS[i].InputCallback();

                    if(!iS[i].permanent) {
                        iS.RemoveAt(i);
                    }
                }
            }
    }

    public void AddNewInput<T>(IInputCallback<T> iCB, T cbParam, KeyCode key, int iT, bool perm = false) {
        iS.Add(new InputData<T>(cbParam, iCB, key, iT, perm));
    }

    public void RunOnStart() {
        iS = new List<InputDataBase>();
    }

    public void RunOnCreated() {
        iS = new List<InputDataBase>();
    }
}
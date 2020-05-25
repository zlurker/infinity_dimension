using System.Collections.Generic;
using UnityEngine;

public enum InputPressType {
    DOWN, HOLD, UP
}

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

    //public EnhancedList<InputDataBase> iS;
    public Dictionary<int, Dictionary<int, List<InputDataBase>>> keyEvents;

    void Update() {
        foreach(var kE in keyEvents)
            foreach(var e in kE.Value) 
                TriggerKeyEvent(kE.Key, e.Key);
    }

    public void TriggerKeyEvent(int key, int inputType) {

        if(!keyEvents.ContainsKey(key) || !keyEvents[key].ContainsKey(inputType))
            return;

        for(int i = keyEvents[key][inputType].Count - 1; i >= 0; i--) {
            bool inputTriggered = false;
            KeyCode keyCode = (KeyCode)key;
            InputPressType typeEnum = (InputPressType)inputType;

            switch(typeEnum) {
                case InputPressType.DOWN:
                    if(Input.GetKeyDown(keyCode))
                        inputTriggered = true;
                    break;

                case InputPressType.HOLD:
                    if(Input.GetKey(keyCode))
                        inputTriggered = true;
                    break;

                case InputPressType.UP:
                    if(Input.GetKeyUp(keyCode))
                        inputTriggered = true;
                    break;
            }

            if(inputTriggered) {
                keyEvents[key][inputType][i].InputCallback();

                if(!keyEvents[key][inputType][i].permanent)
                    keyEvents[key][inputType].RemoveAt(i);
            }
        }
    }

    public void AddNewInput<T>(IInputCallback<T> iCB, T cbParam, KeyCode key, InputPressType iT, bool perm = false) {

        int kId = (int)key;
        int inputType = (int)iT;

        if(!keyEvents.ContainsKey(kId))
            keyEvents.Add(kId, new Dictionary<int, List<InputDataBase>>());

        if(!keyEvents[kId].ContainsKey((int)iT))
            keyEvents[kId].Add(inputType, new List<InputDataBase>());

        keyEvents[kId][inputType].Add(new InputData<T>(cbParam, iCB, key, inputType, perm));
        //return iS.Add(new InputData<T>(cbParam, iCB, key, iT, perm));
    }

    /*public void RemoveInput(int index) {
        iS.Remove(index);
    }*/

    public void RunOnStart() {
        keyEvents.Clear();
        //iS = new EnhancedList<InputDataBase>();
    }

    public void RunOnCreated() {
        keyEvents = new Dictionary<int, Dictionary<int, List<InputDataBase>>>();
        //iS = new EnhancedList<InputDataBase>();
    }
}
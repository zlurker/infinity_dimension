using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputData {
    public List<DH> rs;
    public KeyCode k; //keycode
    public int b; //button
    public int iT; //inputType

    public InputData(KeyCode key, string id, int inputType) {
        n = id;
        k = key;
        rs = new List<DH>();
        iT = inputType;
    }

    public InputData(int button, string id, int inputType) {
        n = id;
        b = button;
        rs = new List<DH>();
        iT = inputType;
    }
}

public class PlayerInput : MonoBehaviour,ISingleton {

    public List<InputData> iS;

    void Update() {
        for (int i = 0; i < iS.Count; i++) {
            bool inputCheck = false;

            switch (iS[i].n[0]) {
                case 'K':
                    switch (iS[i].iT) {
                        case 0:
                            if (Input.GetKeyDown(iS[i].k))
                                inputCheck = true;
                            break;

                        case 1:
                            if (Input.GetKey(iS[i].k))
                                inputCheck = true;
                            break;

                        case 2:
                            if (Input.GetKeyUp(iS[i].k))
                                inputCheck = true;
                            break;
                    }

                    break;

                case 'M':
                    switch (iS[i].iT) {
                        case 0:
                            if (Input.GetMouseButtonDown(iS[i].b))
                                inputCheck = true;
                            break;

                        case 1:
                            if (Input.GetMouseButton(iS[i].b))
                                inputCheck = true;
                            break;

                        case 2:
                            if (Input.GetMouseButtonUp(iS[i].b))
                                inputCheck = true;
                            break;
                    }

                    break;
            }

            if (inputCheck)
                for (int j = 0; j < iS[i].rs.Count; j++)
                    iS[i].rs[j].Invoke();
        }
    }

    public void AddNewInput(KeyCode key, DH reference, int inputType) {
        string k = "K" + ((int)key).ToString();

        iS[ProcessNewInput(k, new InputData(key, k, inputType))].rs.Add(reference);
    }

    public void AddNewInput(int button, DH reference, int inputType) {
        string k = "M" + button;

        iS[ProcessNewInput(k, new InputData(button, k, inputType))].rs.Add(reference);
    }

    int ProcessNewInput(string key, InputData inputData) {
        int i = Iterator.ReturnKey(iS.ToArray(), key);

        if (i == -1) {
            iS.Add(inputData);
            i = iS.Count - 1;
        }

        return i;
    }

    public void RunOnStart() {
        iS = new List<InputData>();
    }

    public void RunOnCreated() {
        iS = new List<InputData>();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {

    public static PlayerInput i;
    public List<InputData> iS;

    void Awake() {
        i = this;
        DontDestroyOnLoad(gameObject);
        iS = new List<InputData>();

        AddNewInput(KeyCode.L, Load);
    }

    void Update() { //Buggy somewhat. Produces error when switching between scenes
        for (int i = 0; i < iS.Count; i++)
            switch (iS[i].n[0]) {
                case 'K':
                    if (Input.GetKeyDown(iS[i].k))
                        for (int j = 0; j < iS[i].rs.Count; j++)
                            iS[i].rs[j]();
                    break;

                case 'M':
                    if (Input.GetMouseButtonDown(iS[i].b))
                        for (int j = 0; j < iS[i].rs.Count; j++)
                            iS[i].rs[j]();
                    break;
            }
    }

    public void AddNewInput(KeyCode key, r reference) {
        string k = "K" + ((int)key).ToString();

        iS[ProcessNewInput(k, new InputData(key, k))].rs.Add(reference);
    }

    public void AddNewInput(int button, r reference) {
        string k = "M" + button;

        iS[ProcessNewInput(k, new InputData(button, k))].rs.Add(reference);
    }

    int ProcessNewInput(string key, InputData inputData) {
        int i = GlobalData.IterateKey(iS.ToArray(), key);

        if (i == -1) {
            iS.Add(inputData);
            i = iS.Count - 1;
        }

        return i;
    }

    void Load() {
        GlobalData.LoadNewLevel(1);
    }
}

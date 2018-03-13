using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MidLoad : MonoBehaviour {

	void Awake () {
        for (int i = 0; i < LoadedData.sL.Length; i++)
            LoadedData.sL[i].RunOnStart(); //Runs all the singleton start

        SceneManager.LoadScene(SceneTransitionData.sI);
	}
}

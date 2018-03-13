using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandler : MonoBehaviour {

	void Start () {
        LoadedData.uL[0].LoadUI();
        Debug.Log(LoadedData.uL[0].GetType().Name);
        LoadedData.uL[0].LoadUI();
    }
	
	void Update () {
		
	}
}

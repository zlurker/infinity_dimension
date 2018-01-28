using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandler : MonoBehaviour {

	void Start () {
        GlobalData.uL[0].LoadUI();
        Debug.Log(GlobalData.uL[0].GetType().Name);
        GlobalData.uL[0].LoadUI();
    }
	
	void Update () {
		
	}
}

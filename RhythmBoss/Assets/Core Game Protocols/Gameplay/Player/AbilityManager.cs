using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DelegatePools.jD.Add(FireAbility);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FireAbility(int g) {
        Debug.Log(PresetGameplayData.jRT[g].name);
    }
}

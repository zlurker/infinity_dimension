using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNetworkDataCompiler : MonoBehaviour, ISingleton {

    public List<AbilityCentralThreadPool> pendingData;
	
	// Update is called once per frame
	void Update () {
		
        if (pendingData.Count > 0) {
            for(int i = 0; i < pendingData.Count; i++)
                pendingData[i].CompileVariableNetworkData();

            pendingData.Clear();
        }
	}

    public void RunOnCreated() {
        pendingData = new List<AbilityCentralThreadPool>();
    }

    public void RunOnStart() {
        
    }
}

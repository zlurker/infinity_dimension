using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class AbilityCalibrator : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
        FileSaveTemplate fST = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; });

        string[] cData = fST.GenericLoadAll(0);

        for(int i = 0; i < cData.Length; i++) {
            //SavedDataCommit[] prevDataCommit = JsonConvert.DeserializeObject<SavedDataCommit[]>(cData[i]);
            //SavedData[] prevData = SavedData.CreateLoadFile(prevDataCommit);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

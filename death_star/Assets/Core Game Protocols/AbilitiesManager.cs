using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour {

    AbilityDataSubclass[] ability;
    int[] rootSubclasses;

    void Start () {

        //Deserializes ability.
        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { "4" }, 0);
        ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData));

        //Deserializes root classes.
        cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { "4" }, 3);
        rootSubclasses = JsonConvert.DeserializeObject<int[]>(cData);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

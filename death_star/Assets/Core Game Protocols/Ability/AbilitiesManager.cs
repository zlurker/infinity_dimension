using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour {

    AbilityDataSubclass[] ability;
    int[] rootSubclasses;

    void Start () {

        //Deserializes ability.
        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { "2" }, 0);
        ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData));

        //Deserializes root classes.
        cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { "2" }, 3);
        rootSubclasses = JsonConvert.DeserializeObject<int[]>(cData);

        TreeTransverser defaultTransverser = new TreeTransverser();
        int tId = TreeTransverser.globalList.Add(defaultTransverser);

        AbilityTreeNode[] a = new AbilityTreeNode[ability.Length];

        for (int i =0; i < a.Length; i++) {
            a[i] = Spawner.GetCType(Singleton.GetSingleton<Spawner>().CreateScriptedObject(new System.Type[] { ability[i].classType }),ability[i].classType) as AbilityTreeNode;
            a[i].runtimeParameters = ability[i].var;
            a[i].treeTransverser = tId;
        }

        defaultTransverser.abilityNodes = AbilityTreeNode.globalList.Add(a);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

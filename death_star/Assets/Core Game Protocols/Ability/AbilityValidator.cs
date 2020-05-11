/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class AbilityValidator {

    public static void ValidateAbilities() {
        string[] datafiles = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(0);

        for(int i = 0; i < datafiles.Length; i++) {

            AbilityDataSubclass[] aData = LoadedData.GetSingleton<JSONFileConvertor>().ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(datafiles[i]));
            Dictionary<Tuple<int, int>, int> relinkRequired = new Dictionary<Tuple<int, int>, int>();

            // First loop to deal with variable switching and finding relinks. 
            for(int j = 0; j < aData.Length; j++) {
                RuntimeParameters[] newRps = LoadedData.ReturnNodeVariables(aData[j].classType);
                Variable[] newVar = new Variable[newRps.Length];

                for(int k = 0; k < newVar.Length; k++)
                    newVar[k] = new Variable(newRps[k]);


                Debug.Log(aData[j].var.Length);
                Debug.LogFormat("Original: {0}", aData[j].var[0].field.n);

                for(int k = 0; k < aData[j].var.Length; k++) {

                    int newIndex = -1;

                    if(LoadedData.loadedParamInstances[aData[j].classType].variableAddresses.ContainsKey(aData[j].var[k].field.n)) {
                        newIndex = LoadedData.loadedParamInstances[aData[j].classType].variableAddresses[aData[j].var[k].field.n];
                        newVar[newIndex] = aData[j].var[k];
                    } else
                        Debug.Log("Variable has been removed.");

                    if(newIndex != k) {
                        relinkRequired.Add(Tuple.Create(j, k), newIndex);
                        Debug.Log(aData[j].var[k].field.n + " has been shifted.");
                    }
                }

                aData[j].var = newVar;
            }

            for(int j = 0; j < aData.Length; j++)
                for(int k = 0; k < aData[j].var.Length; k++) {
                    List<int[]> links = new List<int[]>(aData[j].var[k].links);

                    for(int f = links.Count - 1; f >= 0; f--) {

                        Tuple<int, int> id = Tuple.Create(links[f][0], links[f][1]);

                        if(relinkRequired.ContainsKey(id))
                            if(relinkRequired[id] > -1) {
                                Debug.Log(links[f][1] + " id variable of " + aData[j].var[k].field.n + " has been moved to " + relinkRequired[id]);
                                links[f][1] = relinkRequired[id];
                            } else
                                links.RemoveAt(f);
                    }
                }
        }
    }

    // Use this for initialization
    void Start() {



    }

    // Update is called once per frame
    void Update() {

    }
}
*/
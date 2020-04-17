using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class StandardJSONFileFormat {
    public Type cT;
    public string[] rP;
    public string[] l;
    public string[] vN;
    //public int[] vT; //variableType

    public StandardJSONFileFormat() {

    }

    public StandardJSONFileFormat(int varLen) {
        rP = new string[varLen];
        l = new string[varLen];
        vN = new string[varLen];
    }
}

public class JSONFileConvertor : MonoBehaviour, IRPGeneric, ISingleton {

    StandardJSONFileFormat[] standardFiles;
    AbilityDataSubclass[] convertedFormat;

    public StandardJSONFileFormat[] ConvertToStandard(AbilityDataSubclass[] aDS) {

        standardFiles = new StandardJSONFileFormat[aDS.Length];
        convertedFormat = aDS;

        for(int i = 0; i < convertedFormat.Length; i++) {
            standardFiles[i] = new StandardJSONFileFormat(aDS[i].var.Length);
            standardFiles[i].cT = aDS[i].classType;
            //convertedFormat[i].wL = aDS[i].wL;

            for(int j = 0; j < aDS[i].var.Length; j++) {
                standardFiles[i].rP[j] = aDS[i].var[j].field.GetSerializedObject();
                standardFiles[i].l[j] = JsonConvert.SerializeObject(aDS[i].var[j].links);
                standardFiles[i].vN[j] = aDS[i].var[j].field.n;
                //convertedFormat[i].vT[j] = aDS[i].var[j].field.vI;            
            }
        }

        return standardFiles;
    }

    public AbilityDataSubclass[] ConvertToData(StandardJSONFileFormat[] sFs) {

        convertedFormat = null;
        standardFiles = sFs;

        if(sFs != null) {
            convertedFormat = new AbilityDataSubclass[sFs.Length];
            Dictionary<Tuple<int, int>, int> relinkRequired = new Dictionary<Tuple<int, int>, int>();

            for(int i = 0; i < convertedFormat.Length; i++) {
                convertedFormat[i] = new AbilityDataSubclass();

                convertedFormat[i].classType = sFs[i].cT;
                convertedFormat[i].var = new Variable[LoadedData.loadedParamInstances[convertedFormat[i].classType].runtimeParameters.Length];

                for(int j = 0; j < standardFiles[i].rP.Length; j++) {
                    int varId = -1;

                    if(LoadedData.loadedParamInstances[convertedFormat[i].classType].variableAddresses.ContainsKey(sFs[i].vN[j])) {
                        varId = LoadedData.loadedParamInstances[convertedFormat[i].classType].variableAddresses[sFs[i].vN[j]];
                        LoadedData.loadedParamInstances[convertedFormat[i].classType].runtimeParameters[varId].rP.RunGenericBasedOnRP(this, new int[] { i, j, varId });
                    } else
                        Debug.Log("Variable has been removed.");

                    if(varId != j) {
                        relinkRequired.Add(Tuple.Create(i, j), varId);
                        Debug.Log(sFs[i].vN[j] + " has been shifted.");
                    }
                }

                for(int j = 0; j < convertedFormat[i].var.Length; j++)
                    if(convertedFormat[i].var[j] == null)
                        convertedFormat[i].var[j] = new Variable(LoadedData.loadedParamInstances[convertedFormat[i].classType].runtimeParameters[j].rP);

            }

            for(int i = 0; i < convertedFormat.Length; i++)
                for(int j = 0; j < convertedFormat[i].var.Length; j++) {
                    List<int[]> links = new List<int[]>(convertedFormat[i].var[j].links);

                    for(int k = links.Count - 1; k >= 0; k--) {

                        Tuple<int, int> id = Tuple.Create(links[k][0], links[k][1]);

                        if(relinkRequired.ContainsKey(id))
                            if(relinkRequired[id] > -1) {
                                Debug.Log(links[k][1] + " id variable of " + convertedFormat[j].var[k].field.n + " has been moved to " + relinkRequired[id]);
                                links[k][1] = relinkRequired[id];
                            } else
                                links.RemoveAt(k);
                    }

                    convertedFormat[i].var[j].links = links.ToArray();
                }
        }

        return convertedFormat;
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        int subclass = ((int[])(object)arg)[0];
        int oldId = ((int[])(object)arg)[1];
        int newId = ((int[])(object)arg)[2];

        RuntimeParameters<T> rP = null;

        try {
            rP = JsonConvert.DeserializeObject<RuntimeParameters<T>>(standardFiles[subclass].rP[oldId]);
        }catch (Exception e) {
            Debug.Log("Could not convert. Reverting to source.");
        }

        int[][] links = JsonConvert.DeserializeObject<int[][]>(standardFiles[subclass].l[oldId]);

        if(rP != null)
            convertedFormat[subclass].var[newId] = new Variable(rP, links);
        else
            convertedFormat[subclass].var[newId] = new Variable(LoadedData.loadedParamInstances[convertedFormat[subclass].classType].runtimeParameters[newId].rP, links);
    }

    public void RunOnStart() {

    }

    public void RunOnCreated() {

    }
}

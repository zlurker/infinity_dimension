using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class StandardJSONFileFormat {    
    public Type cT;
    public float[] wL;
    public string[] rP;
    public string[] l;
    public int[] vT; //variableType
}

public static class JSONFileConvertor {

    public static StandardJSONFileFormat[] ConvertToStandard(AbilityDataSubclass[] aDS) {

        StandardJSONFileFormat[] convertedFormat = new StandardJSONFileFormat[aDS.Length];

        for (int i =0; i < convertedFormat.Length; i++) {
            convertedFormat[i] = new StandardJSONFileFormat();
            convertedFormat[i].cT = aDS[i].classType;
            convertedFormat[i].wL = aDS[i].wL;

            convertedFormat[i].rP = new string[aDS[i].var.Length];
            convertedFormat[i].l = new string[aDS[i].var.Length];
            convertedFormat[i].vT = new int[aDS[i].var.Length];

            for (int j =0; j < aDS[i].var.Length; j++) {
                convertedFormat[i].rP[j] = aDS[i].var[j].field.GetSerializedObject();
                convertedFormat[i].l[j] = JsonConvert.SerializeObject(aDS[i].var[j].links);
                convertedFormat[i].vT[j] = aDS[i].var[j].field.vI;
            }
        }

        return convertedFormat;
    }

    public static AbilityDataSubclass[] ConvertToData(StandardJSONFileFormat[] sFs) {

        AbilityDataSubclass[] convertedFormat = new AbilityDataSubclass[sFs.Length];

        for(int i = 0; i < convertedFormat.Length; i++) {
            convertedFormat[i] = new AbilityDataSubclass();

            convertedFormat[i].classType = sFs[i].cT;
            convertedFormat[i].wL = sFs[i].wL;
            convertedFormat[i].var = new Variable[sFs[i].l.Length];

            for(int j = 0; j < convertedFormat[i].var.Length; j++) 
                convertedFormat[i].var[j] = new Variable(VariableTypeIndex.ReturnRuntimeType(sFs[i].vT[j], sFs[i].rP[j]), JsonConvert.DeserializeObject<int[][]>(sFs[i].l[j]));       
        }

        return convertedFormat;
    }
}

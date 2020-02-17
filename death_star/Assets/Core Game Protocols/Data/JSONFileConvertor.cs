using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class StandardJSONFileFormat {
    public Type cT;
    public string[] rP;
    public string[] l;
    public int[] vT; //variableType
}

public static class JSONFileConvertor {

    public static StandardJSONFileFormat[] ConvertToStandard(AbilityDataSubclass[] aDS) {

        StandardJSONFileFormat[] convertedFormat = new StandardJSONFileFormat[aDS.Length];

        for(int i = 0; i < convertedFormat.Length; i++) {
            convertedFormat[i] = new StandardJSONFileFormat();
            convertedFormat[i].cT = aDS[i].classType;
            //convertedFormat[i].wL = aDS[i].wL;

            convertedFormat[i].rP = new string[aDS[i].var.Length];
            convertedFormat[i].l = new string[aDS[i].var.Length];
            convertedFormat[i].vT = new int[aDS[i].var.Length];

            for(int j = 0; j < aDS[i].var.Length; j++) {
                convertedFormat[i].rP[j] = aDS[i].var[j].field.GetSerializedObject();
                convertedFormat[i].l[j] = JsonConvert.SerializeObject(aDS[i].var[j].links);
                convertedFormat[i].vT[j] = aDS[i].var[j].field.vI;
            }
        }

        return convertedFormat;
    }

    public static AbilityDataSubclass[] ConvertToData(StandardJSONFileFormat[] sFs) {

        AbilityTreeNode[] interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];

        AbilityDataSubclass[] convertedFormat = new AbilityDataSubclass[sFs.Length];

        for(int i = 0; i < convertedFormat.Length; i++) {
            convertedFormat[i] = new AbilityDataSubclass();

            convertedFormat[i].classType = sFs[i].cT;
            convertedFormat[i].var = new Variable[sFs[i].l.Length];

            for(int j = 0; j < convertedFormat[i].var.Length; j++) {
                if(sFs[i].vT[j] > -1) {
                    convertedFormat[i].var[j] = new Variable(VariableTypeIndex.convertors[sFs[i].vT[j]].ReturnRuntimeType(sFs[i].rP[j]), JsonConvert.DeserializeObject<int[][]>(sFs[i].l[j]));
                } else {
                    int selectedC = Iterator.ReturnKey<AbilityTreeNode, Type>(interfaces, convertedFormat[i].classType, (p) => { return p.GetType(); });
                    RuntimeParameters inst = interfaces[selectedC].GetRuntimeParameters()[j];
                    convertedFormat[i].var[j] = new Variable(inst, JsonConvert.DeserializeObject<int[][]>(sFs[i].l[j]));
                }
            }
        }


        return convertedFormat;
    }
}

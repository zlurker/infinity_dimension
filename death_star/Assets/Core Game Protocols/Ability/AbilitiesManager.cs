using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilitiesManager : MonoBehaviour {

    public class AbilityData {
        Variable[][] dataVar;
        Type[] dataType;
        int[] rootSubclasses;
        int[] lengthData;

        public AbilityData(Variable[][] dV, Type[] dT, int[] rS, int[] lD) {
            dataVar = dV;
            dataType = dT;
            rootSubclasses = rS;
            lengthData = lD;
        }

        public void CreateAbility(object[] p) {
            TreeTransverser defaultTransverser = new TreeTransverser();
            int tId = TreeTransverser.globalListTree.Add(defaultTransverser);

            AbilityTreeNode[] a = new AbilityTreeNode[dataVar.Length];
            int nId = AbilityTreeNode.globalList.Add(a);

            defaultTransverser.SetVariableNetworkData(dataVar, dataType,tId);
            defaultTransverser.SetNodeData(nId, lengthData, rootSubclasses, rootSubclasses.Length);
            defaultTransverser.SetTransverserId(tId);
            defaultTransverser.StartTreeTransverse();
        }
    }

    AbilityData[] aData;

    void Start() {
        string[] abilityNodeData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(0);
        string[] abilityRootData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(3);
        string[] abilityEndData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(4);

        aData = new AbilityData[abilityNodeData.Length];

        for(int i = 0; i < abilityNodeData.Length; i++) {
            AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData[i]));
            int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData[i]);
            int[] lengthData = JsonConvert.DeserializeObject<int[]>(abilityEndData[i]);

            Variable[][] tempVar = new Variable[ability.Length][];
            Type[] tempTypes = new Type[ability.Length];

            for(int j = 0; j < ability.Length; j++) {
                tempVar[j] = ability[j].var;
                tempTypes[j] = ability[j].classType;
                //a[i] = Spawner.GetCType(Singleton.GetSingleton<Spawner>().CreateScriptedObject(new System.Type[] { ability[i].classType }), ability[i].classType) as AbilityTreeNode;
                //a[i].gameObject.SetActive(false);
                //a[i].RunNodeInitialisation(i, tId);
            }

            aData[i] = new AbilityData(tempVar, tempTypes, rootSubclasses, lengthData);
            Singleton.GetSingleton<PlayerInput>().AddNewInput((KeyCode)97 + i, new DH(aData[i].CreateAbility), 0);
        }

        //Deserializes ability.
        //string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { "0" }, 0);


        //Deserializes root classes.        







    }
}

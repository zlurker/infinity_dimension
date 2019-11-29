using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class AbilitiesManager : MonoBehaviour {

    public static EnhancedList<ScriptableObject> defaultTreeTransversers;

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
            ScriptableObject treeObject = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { typeof(TreeTransverser) });
            TreeTransverser defaultTransverser = Spawner.GetCType<TreeTransverser>(treeObject);
            int tId = TreeTransverser.globalListTree.Add(defaultTransverser);

            ScriptableObject[] a = new ScriptableObject[dataVar.Length];
            int nId = AbilityTreeNode.globalList.Add(a);

            int dId = defaultTreeTransversers.Add(treeObject);

            defaultTransverser.SetRootTransverserData(dataVar, dataType,tId, dId);
            defaultTransverser.SetNodeData(nId, lengthData, rootSubclasses, rootSubclasses.Length);
            defaultTransverser.SetTransverserId(tId);
            defaultTransverser.BeginNodeCallback();
        }
    }

    AbilityData[] aData;

    void Start() {

        defaultTreeTransversers = new EnhancedList<ScriptableObject>();

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
            }

            aData[i] = new AbilityData(tempVar, tempTypes, rootSubclasses, lengthData);
            Singleton.GetSingleton<PlayerInput>().AddNewInput((KeyCode)97 + i, new DH(aData[i].CreateAbility), 0);
        }
    }

    public static void RemoveExpiredTree(int id) {
        Singleton.GetSingleton<Spawner>().Remove(defaultTreeTransversers.l[id]);
        defaultTreeTransversers.Remove(id);
    }
}

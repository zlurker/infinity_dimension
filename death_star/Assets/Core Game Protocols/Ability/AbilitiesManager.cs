using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class AbilitiesManager : MonoBehaviour {

    //public static EnhancedList<ScriptableObject> defaultTreeTransversers;

    public class AbilityData {
        Variable[][] dataVar;
        Type[] dataType;
        int[] rootSubclasses;
        int[] nodeType;
        int[] nodeBranchingData;
        Dictionary<int, int> specialisedNodeData;
        int abilityId;

        public AbilityData(Variable[][] dV, Type[] dT, int[] rS, int[] nT, int[] nBD, Dictionary<int, int> sND, int aId) {
            dataVar = dV;
            dataType = dT;
            rootSubclasses = rS;
            nodeType = nT;
            nodeBranchingData = nBD;
            specialisedNodeData = sND;
            abilityId = aId;
        }

        public void SyncInputWithNetwork(object[] p) {
            if(ClientProgram.clientInst)
                ClientProgram.clientInst.CombineChannelWithMessage(ClientProgram.ASYNC_INPUT, abilityId.ToString());
             else
                CreateAbility(null);
        }

        public void CreateAbility(object[] p) {

            TravelThread centralPool = new TravelThread();
            int tId = TravelThread.globalCentralList.Add(centralPool);

            ScriptableObject[] a = new ScriptableObject[dataVar.Length];
            int nId = AbilityTreeNode.globalList.Add(a);

            // Rather than create new instance, everything except variables will be taken from here.
            centralPool.SetCentralData(tId, nId, dataVar, dataType, rootSubclasses, nodeBranchingData, nodeType, specialisedNodeData);
            centralPool.StartThreads();
        }
    }


public static AbilityData[] aData;

void Start() {

    //defaultTreeTransversers = new EnhancedList<ScriptableObject>();
    string[] abilityNodeData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(0);
    string[] abilityRootData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(3);
    string[] abilityNodeBranchingData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(4);
    string[] abilitySpecialisedData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(5);

    aData = new AbilityData[abilityNodeData.Length];

    for(int i = 0; i < abilityNodeData.Length; i++) {
        AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData[i]));
        int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData[i]);
        int[] nodeBranchData = JsonConvert.DeserializeObject<int[]>(abilityNodeBranchingData[i]);
        Dictionary<int, int> specialisedNodeData = JsonConvert.DeserializeObject<Dictionary<int, int>>(abilitySpecialisedData[i]);

        Variable[][] tempVar = new Variable[ability.Length][];
        Type[] tempTypes = new Type[ability.Length];

        for(int j = 0; j < ability.Length; j++) {
            tempVar[j] = ability[j].var;
            tempTypes[j] = ability[j].classType;
        }

        int[] nodeType = new int[ability.Length];

        aData[i] = new AbilityData(tempVar, tempTypes, rootSubclasses, nodeType, nodeBranchData, specialisedNodeData, i);
        Singleton.GetSingleton<PlayerInput>().AddNewInput((KeyCode)97 + i, new DH(aData[i].SyncInputWithNetwork), 0);
    }
}
}

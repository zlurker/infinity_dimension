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
        int[] lengthData;
        int[] nodeType;
        int[] nodeBranchingData;
        Dictionary<int, int> specialisedNodeData;

        public AbilityData(Variable[][] dV, Type[] dT, int[] rS, int[] lD, int[] nT, int[] nBD, Dictionary<int, int> sND) {
            dataVar = dV;
            dataType = dT;
            rootSubclasses = rS;
            lengthData = lD;
            nodeType = nT;
            nodeBranchingData = nBD;
            specialisedNodeData = sND;
        }

        public void CreateAbility(object[] p) {
            TravelThread centralPool = new TravelThread();
            int tId = TravelThread.globalCentralList.Add(centralPool);

            ScriptableObject[] a = new ScriptableObject[dataVar.Length];
            int nId = AbilityTreeNode.globalList.Add(a);

            // Rather than create new instance, everything except variables will be taken from here.
            centralPool.SetCentralData(tId, dataVar, dataType, rootSubclasses, nodeBranchingData, nodeType,specialisedNodeData);
            centralPool.StartThreads();

            //int dId = defaultTreeTransversers.Add(treeObject);

            //defaultTransverser.SetRootTransverserData(dataVar, dataType,tId, dId);
            // defaultTransverser.SetNodeData(nId, lengthData, rootSubclasses, nodeType);
            //defaultTransverser.SetTransverserId(tId);
            //defaultTransverser.BeginNodeCallback();
        }
    }

    AbilityData[] aData;

    void Start() {

        //defaultTreeTransversers = new EnhancedList<ScriptableObject>();

        string[] abilityNodeData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(0);
        string[] abilityRootData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(3);
        string[] abilityEndData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(4);
        //string[] abilityGetEndData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(5);
        string[] abilityNodeBranchingData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(5);
        string[] abilitySpecialisedData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadAll(6);

        aData = new AbilityData[abilityNodeData.Length];

        for(int i = 0; i < abilityNodeData.Length; i++) {
            AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData[i]));
            int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData[i]);
            int[] lengthData = JsonConvert.DeserializeObject<int[]>(abilityEndData[i]);
            //int[][] getEndData = JsonConvert.DeserializeObject<int[][]>(abilityGetEndData[i]);
            int[] nodeBranchData = JsonConvert.DeserializeObject<int[]>(abilityNodeBranchingData[i]);
            Dictionary<int, int> specialisedNodeData = JsonConvert.DeserializeObject<Dictionary<int, int>>(abilitySpecialisedData[i]);

            Variable[][] tempVar = new Variable[ability.Length][];
            Type[] tempTypes = new Type[ability.Length];

            for(int j = 0; j < ability.Length; j++) {
                tempVar[j] = ability[j].var;
                tempTypes[j] = ability[j].classType;
            }

            int[] nodeType = new int[ability.Length];

            /*for(int j = 0; j < getEndData.Length; j++) {
                nodeType[getEndData[j][0]] = 1;

                int[][] temp = new int[tempVar[getEndData[j][0]][getEndData[j][1]].links[1].Length + 1][];

                for(int k = 0; k < tempVar[getEndData[j][0]][getEndData[j][1]].links[1].Length; k++)
                    temp[k] = tempVar[getEndData[j][0]][getEndData[j][1]].links[1][k];

                temp[temp.Length - 1] = new int[] { getEndData[j][2], getEndData[j][3] };
                tempVar[getEndData[j][0]][getEndData[j][1]].links[1] = temp;
            }*/

            aData[i] = new AbilityData(tempVar, tempTypes, rootSubclasses, lengthData, nodeType, nodeBranchData, specialisedNodeData);
            Singleton.GetSingleton<PlayerInput>().AddNewInput((KeyCode)97 + i, new DH(aData[i].CreateAbility), 0);
        }
    }

    public static void RemoveExpiredTree(int id) {
        //Singleton.GetSingleton<Spawner>().Remove(defaultTreeTransversers.l[id]);
        //defaultTreeTransversers.Remove(id);
    }
}

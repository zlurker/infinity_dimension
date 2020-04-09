using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class AbilityData : IInputCallback<int> {
    // Data that needs to be read/written
    Variable[][] dataVar;
    Type[] dataType;
    AbilityBooleanData boolData;

    // Data that will purely only be read.
    string[] description;
    int[] rootSubclasses;
    int[] nodeBranchingData;
    Dictionary<Tuple<int, int>, int[][]> gData;
    int abilityId;

    public AbilityData(AbilityDataSubclass[] data, int aId) {

        // Sorts variable out accordingly.
        dataVar = new Variable[data.Length][];
        dataType = new Type[data.Length];

        for(int i = 0; i < data.Length; i++) {
            dataVar[i] = data[i].var;
            dataType[i] = data[i].classType;
        }

        abilityId = aId;
        BeginDepenciesBuild();
    }

    void BeginDepenciesBuild() {

        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(dataVar.Length);

        nodeBranchingData = new int[dataVar.Length];
        boolData = new AbilityBooleanData(dataVar);
        gData = new Dictionary<Tuple<int, int>, int[][]>();

        for(int i = 0; i < dataVar.Length; i++) {
            for(int j = 0; j < dataVar[i].Length; j++) {

                List<int> retThreads = new List<int>();

                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    // Marks target as true so it can't be root.
                    connected.ModifyElementAt(dataVar[i][j].links[k][0], true);

                    // Marks target as true so it will be blocked.
                    if(dataVar[i][j].field.t == dataVar[currLink[0]][currLink[1]].field.t)
                        boolData.varsBlocked[currLink[0]][currLink[1]] = true;

                    // If return, add to return threads
                    if(dataType[currLink[0]] == typeof(ReturnValue))
                        retThreads.Add(j);
                }

                // Manages the swapping of threads. Removes links that are to be blocked.
                if(retThreads.Count > 0) {
                    List<int[]> listToProcess = new List<int[]>(dataVar[i][j].links);
                    int[][] subLinks = new int[retThreads.Count][];

                    for(int k = retThreads.Count - 1; k >= 0; k--) {
                        subLinks[k] = listToProcess[retThreads[k]];
                        listToProcess.RemoveAt(retThreads[k]);
                    }

                    dataVar[i][j].links = subLinks;
                    gData.Add(Tuple.Create<int, int>(i, j), listToProcess.ToArray());
                }

                // Gets the sum of branches.
                nodeBranchingData[i] += dataVar[i][j].links.Length;
            }
        }

        List<int> rC = new List<int>();

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rC.Add(i);

        rootSubclasses = rC.ToArray();
    }

    public void InputCallback(int i) {
        AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool(ClientProgram.clientId);
        CreateAbility(centralPool);

        if(ClientProgram.clientInst) {
            AbilityNodeNetworkData[] data = centralPool.GetVariableNetworkData();

            AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
            encoder.SendInputSignal(centralPool, abilityId, data);
        }
    }

    public void CreateAbility(AbilityCentralThreadPool threadInst) {

        int tId = AbilityCentralThreadPool.globalCentralList.Add(threadInst);

        AbilityTreeNode[] a = new AbilityTreeNode[dataVar.Length];
        int nId = AbilityTreeNode.globalList.Add(new AbilityNodeHolder(tId.ToString(), a));
        Variable[][] clonedCopy = CloneRuntimeParams(dataVar);
        bool[][] clonedBoolValues = boolData.ReturnNewCopy();

        // Rather than create new instance, everything except variables will be taken from here.
        threadInst.SetCentralData(tId, nId, clonedCopy, dataType, rootSubclasses, nodeBranchingData, clonedBoolValues, gData);
        threadInst.StartThreads();
        //threadInst.SendVariableNetworkData();
    }

    Variable[][] CloneRuntimeParams(Variable[][] target) {
        Variable[][] clonedCopy = new Variable[target.Length][];

        for(int i = 0; i < target.Length; i++) {

            clonedCopy[i] = new Variable[target[i].Length];

            for(int j = 0; j < target[i].Length; j++)
                clonedCopy[i][j] = new Variable(target[i][j].field.ReturnNewRuntimeParamCopy(), target[i][j].links);
        }
        return clonedCopy;
    }
}

public sealed class AbilitiesManager : MonoBehaviour {

    public class PlayerAssetData {
        public AbilityData[] abilties;
        public Dictionary<string, Sprite> assetData;
        public Dictionary<int, int> abilityManifest;

        public PlayerAssetData() {
            assetData = new Dictionary<string, Sprite>();
            abilityManifest = new Dictionary<int, int>();
        }
    }



    public static Dictionary<int, PlayerAssetData> aData;

    void Start() {
        int priCharacterId = aData[ClientProgram.clientId].abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER];

        AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
        aData[ClientProgram.clientId].abilties[priCharacterId].InputCallback(0);
        //encoder.SendInputSignal(priCharacterId,null);
        AssignInputKeys();
    }

    public void AssignInputKeys() {
        if(aData.ContainsKey(ClientProgram.clientId)) {
            int actualKeyLoaded = 0;
            for(int i = 0; i < aData[ClientProgram.clientId].abilties.Length; i++)
                if(!aData[ClientProgram.clientId].abilityManifest.ContainsValue(i)) {
                    LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[ClientProgram.clientId].abilties[i], 0, (KeyCode)(97 + actualKeyLoaded), 0, true);
                    actualKeyLoaded++;
                }
        }
    }

    public static PlayerAssetData GetAssetData(int playerid) {
        if(aData.ContainsKey(playerid))
            return aData[playerid];

        PlayerAssetData inst = new PlayerAssetData();
        aData.Add(playerid, inst);
        return inst;
    }
}

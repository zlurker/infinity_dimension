using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;

public class LinkData {

    public HashSet<Tuple<int, int, int>> lHS;
    public HashSet<Tuple<int, int, int>> rHS;

    public LinkData() {
        lHS = new HashSet<Tuple<int, int, int>>();
        rHS = new HashSet<Tuple<int, int, int>>();
    }
}

public class LinkModifier {

    public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> add;
    public Dictionary<Tuple<int, int>, HashSet<int>> remove;

    public LinkModifier() {
        add = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
        remove = new Dictionary<Tuple<int, int>, HashSet<int>>();
    }

    public void Add(int a1, int a2, int b1, int b2) {
        Tuple<int, int> turpA = Tuple.Create(a1, a2);

        if(!add.ContainsKey(turpA))
            add.Add(turpA, new HashSet<Tuple<int, int>>());

        Tuple<int, int> turpB = Tuple.Create(b1, b2);

        if(!add[turpA].Contains(turpB))
            add[turpA].Add(turpB);
    }

    public void Remove(int a1, int a2, int b1) {
        Tuple<int, int> turpA = Tuple.Create(a1, a2);

        if(!remove.ContainsKey(turpA))
            remove.Add(turpA, new HashSet<int>());

        if(!remove[turpA].Contains(b1))
            remove[turpA].Add(b1);
    }
}

public class AbilityData : IInputCallback<int> {
    // Data that needs to be read/written
    Variable[][] dataVar;
    Type[] dataType;
    AbilityBooleanData boolData;

    // Data that will purely only be read.
    string[] description;
    int[] rootSubclasses;
    int[] nodeBranchingData;
    int abilityId;

    public AbilityData(AbilityDataSubclass[] data, int aId) {

        // Sorts variable out accordingly.
        dataVar = new Variable[data.Length][];
        dataType = new Type[data.Length];
        LinkData[] linkData = new LinkData[data.Length];

        for(int i = 0; i < data.Length; i++) {
            dataVar[i] = data[i].var;
            dataType[i] = data[i].classType;
            linkData[i] = new LinkData();
        }

        abilityId = aId;

        RetrieveStartNodes();

        for(int i = 0; i < rootSubclasses.Length; i++)
            CreateAbilityLinkMap(rootSubclasses[i], linkData);

        EditLinks(linkData);
        BeginDepenciesBuild();
    }

    void RetrieveStartNodes() {
        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(dataVar.Length);

        for(int i = 0; i < dataVar.Length; i++)
            for(int j = 0; j < dataVar[i].Length; j++)
                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    // Marks target as true so it can't be root.
                    connected.ModifyElementAt(currLink[0], true);
                }

        List<int> rC = new List<int>();

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rC.Add(i);

        rootSubclasses = rC.ToArray();
    }

    void CreateAbilityLinkMap(int nextNode, LinkData[] lD) {

        for(int i = 0; i < dataVar[nextNode].Length; i++)
            for(int j = 0; j < dataVar[nextNode][i].links.Length; j++) {
                int[] currLink = dataVar[nextNode][i].links[j];

                // Adds links to rhs.
                Tuple<int, int,int> rhslinkTup = Tuple.Create(currLink[0], currLink[1],j);

                if(!lD[nextNode].rHS.Contains(rhslinkTup))
                    lD[nextNode].rHS.Add(rhslinkTup);

                // Adds links to target lhs.
                Tuple<int, int, int> lhslinkTup = Tuple.Create(nextNode, i, j);

                if(!lD[currLink[0]].lHS.Contains(lhslinkTup))
                    lD[currLink[0]].lHS.Add(lhslinkTup);

                // Iterates to target.
                CreateAbilityLinkMap(currLink[0], lD);
            }
    }

    void EditLinks(LinkData[] lD) {
        LinkModifier lM = new LinkModifier();

        for(int i = 0; i < dataType.Length; i++)
            LoadedData.loadedNodeInstance[dataType[i]].LinkEdit(i, lD, lM,dataVar);

        foreach (var add in lM.add) {

            List<int[]> links = new List<int[]>(dataVar[add.Key.Item1][add.Key.Item2].links);

            foreach(var ele in add.Value)
                links.Add(new int[] { ele.Item1, ele.Item2 });

            dataVar[add.Key.Item1][add.Key.Item2].links = links.ToArray();
        }

        foreach (var rm in lM.remove) {
            int[] rmArr = rm.Value.ToArray();
            Array.Sort(rmArr);

            List<int[]> links = new List<int[]>(dataVar[rm.Key.Item1][rm.Key.Item2].links);

            for(int i = rmArr.Length - 1; i >= 0; i--)
                links.RemoveAt(rmArr[i]);

            dataVar[rm.Key.Item1][rm.Key.Item2].links = links.ToArray();
        }
    }

    void BeginDepenciesBuild() {

        nodeBranchingData = new int[dataVar.Length];
        boolData = new AbilityBooleanData(dataVar);

        for(int i = 0; i < dataVar.Length; i++) {
            for(int j = 0; j < dataVar[i].Length; j++) {

                AutoPopulationList<List<int[]>> varLinks = new AutoPopulationList<List<int[]>>(1);

                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    // Marks target as true so it will be blocked.
                    if(dataVar[i][j].field.t == dataVar[currLink[0]][currLink[1]].field.t)
                        boolData.varsBlocked[currLink[0]][currLink[1]] = true;
                }

                nodeBranchingData[i] += dataVar[i][j].links.Length;
            }
        }
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
        threadInst.SetCentralData(tId, nId, clonedCopy, dataType, rootSubclasses, nodeBranchingData, clonedBoolValues);
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

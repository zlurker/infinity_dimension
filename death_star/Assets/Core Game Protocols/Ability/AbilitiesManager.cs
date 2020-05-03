﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine.UI;

public class LinkData {

    // < NodeConnected, VariableConnected, LinkType, LinkID  >
    public HashSet<Tuple<int, int, int, int>> lHS;
    public HashSet<Tuple<int, int, int, int>> rHS;

    public LinkData() {
        lHS = new HashSet<Tuple<int, int, int, int>>();
        rHS = new HashSet<Tuple<int, int, int, int>>();
    }
}

public class LinkModifier {

    public Dictionary<Tuple<int, int>, HashSet<int>> remove;
    public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> redirects;
    public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int, int>>> add;

    public LinkModifier() {
        remove = new Dictionary<Tuple<int, int>, HashSet<int>>();
        redirects = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
        add = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int, int>>>();
    }

    public void Remove(int a1, int a2, int b1) {
        Tuple<int, int> turpA = Tuple.Create(a1, a2);

        if(!remove.ContainsKey(turpA))
            remove.Add(turpA, new HashSet<int>());

        if(!remove[turpA].Contains(b1))
            remove[turpA].Add(b1);
    }

    public void Redirect(int a1, int a2, int b1, int b2) {
        Tuple<int, int> turpA = Tuple.Create(a1, a2);

        if(!redirects.ContainsKey(turpA))
            redirects.Add(turpA, new HashSet<Tuple<int, int>>());

        Tuple<int, int> turpB = Tuple.Create(b1, b2);

        if(!redirects[turpA].Contains(turpB))
            redirects[turpA].Add(turpB);
    }

    public void Add(int a1, int a2, int b1, int b2, int b3) {
        Tuple<int, int> turpA = Tuple.Create(a1, a2);

        if(!add.ContainsKey(turpA))
            add.Add(turpA, new HashSet<Tuple<int, int, int>>());

        Tuple<int, int, int> turpB = Tuple.Create(b1, b2, b3);

        if(!add[turpA].Contains(turpB))
            add[turpA].Add(turpB);
    }
}

public class AbilityData : IInputCallback<int> {


    // Data that needs to be read/written
    Variable[][] dataVar;
    AbilityBooleanData boolData;

    // Data that will purely only be read.
    public AbilityInfo abilityInfo;

    //Dictionary<Tuple<int, int>, HashSet<int>> onCalledDict;
    Dictionary<int, Dictionary<Type, Dictionary<int, HashSet<int>>>> targettedNodes;
    Type[] dataType;
    int[][] rootSubclasses;
    int[] nodeBranchingData;
    int[][] autoManagedVariables;
    string abilityId;


    // Used during consturction phase.
    LinkData[] linkData;
    LinkModifier lM;
    int currBuildNode;

    public int GetCurrBuildNode() {
        return currBuildNode;
    }

    public LinkData GetLinkData(int id) {
        return linkData[id];
    }

    public LinkModifier GetLinkModifier() {
        return lM;
    }

    public Variable GetVariable(int node, int variable) {
        return dataVar[node][variable];
    }

    public void AddTargettedNode(int a1, int a2, Type subCategory,int b1) {
        Tuple<int, int> id = Tuple.Create<int, int>(a1, a2);

        if(!targettedNodes.ContainsKey(a1))
            targettedNodes.Add(a1, new Dictionary<Type, Dictionary<int, HashSet<int>>>());

        if(!targettedNodes[a1].ContainsKey(subCategory))
            targettedNodes[a1].Add(subCategory,new Dictionary<int, HashSet<int>>());

        if(!targettedNodes[a1][subCategory].ContainsKey(a2))
            targettedNodes[a1][subCategory].Add(a2, new HashSet<int>());

        if(!targettedNodes[a1][subCategory][a2].Contains(b1))
            targettedNodes[a1][subCategory][a2].Add(b1);
    }

    public AbilityData(AbilityDataSubclass[] data, AbilityInfo aD, string aId) {

        abilityInfo = aD;
        // Sorts variable out accordingly.
        dataVar = new Variable[data.Length + 1][];
        dataType = new Type[data.Length + 1];
        linkData = new LinkData[data.Length + 1];
        targettedNodes = new Dictionary<int, Dictionary<Type, Dictionary<int, HashSet<int>>>>();

        for(int i = 0; i < data.Length; i++) {
            dataVar[i] = data[i].var;
            dataType[i] = data[i].classType;
            linkData[i] = new LinkData();
        }

        abilityId = aId;

        RetrieveStartNodes();

        // Adds the psuedo node after the initial calculation.
        dataVar[dataVar.Length - 1] = new Variable[] { new Variable(LoadedData.loadedParamInstances[typeof(NodeThreadStarter)].runtimeParameters[1].rP, rootSubclasses) };
        dataType[dataVar.Length - 1] = typeof(NodeThreadStarter);
        linkData[dataVar.Length - 1] = new LinkData();

        CreateAbilityLinkMap(dataVar.Length - 1);

        EditLinks();
        BeginDepenciesBuild();
    }

    void RetrieveStartNodes() {
        int nonPsuedoNodes = dataVar.Length - 1;

        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(nonPsuedoNodes);

        for(int i = 0; i < nonPsuedoNodes; i++)
            for(int j = 0; j < dataVar[i].Length; j++)
                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    // Marks target as true so it can't be root.
                    connected.ModifyElementAt(currLink[0], true);
                }

        List<int[]> rC = new List<int[]>();

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rC.Add(new int[] { i, 0, 1 });

        rootSubclasses = rC.ToArray();
    }

    void CreateAbilityLinkMap(int nextNode) {

        for(int i = 0; i < dataVar[nextNode].Length; i++)
            for(int j = 0; j < dataVar[nextNode][i].links.Length; j++) {
                int[] currLink = dataVar[nextNode][i].links[j];

                // Adds links to rhs.
                Tuple<int, int, int, int> rhslinkTup = Tuple.Create(currLink[0], currLink[1], currLink[2], j);

                if(!linkData[nextNode].rHS.Contains(rhslinkTup))
                    linkData[nextNode].rHS.Add(rhslinkTup);

                // Adds links to target lhs.
                Tuple<int, int, int, int> lhslinkTup = Tuple.Create(nextNode, i, currLink[2], j);

                if(!linkData[currLink[0]].lHS.Contains(lhslinkTup))
                    linkData[currLink[0]].lHS.Add(lhslinkTup);

                // Iterates to target.
                CreateAbilityLinkMap(currLink[0]);
            }
    }

    void EditLinks() {
        lM = new LinkModifier();

        for(currBuildNode = 0; currBuildNode < dataType.Length; currBuildNode++)
            LoadedData.loadedNodeInstance[dataType[currBuildNode]].ConstructionPhase(this);
        //LoadedData.loadedNodeInstance[dataType[i]].LinkEdit(i, lD, lM, dataVar);

        foreach(var add in lM.add) {

            List<int[]> links = new List<int[]>(dataVar[add.Key.Item1][add.Key.Item2].links);

            foreach(var ele in add.Value) {
                links.Add(new int[] { ele.Item1, ele.Item2, ele.Item3 });
                Debug.LogFormat("Adding to {0},{1}. Content: {2},{3}", add.Key.Item1, add.Key.Item2, ele.Item1, ele.Item2);
            }

            dataVar[add.Key.Item1][add.Key.Item2].links = links.ToArray();
        }

        foreach(var rm in lM.remove) {
            int[] rmArr = rm.Value.ToArray();
            Array.Sort(rmArr);

            List<int[]> links = new List<int[]>(dataVar[rm.Key.Item1][rm.Key.Item2].links);

            for(int i = rmArr.Length - 1; i >= 0; i--) {
                Debug.LogFormat("Removing at {0},{1}. Content: {2},{3}", rm.Key.Item1, rm.Key.Item2, links[rmArr[i]][0], links[rmArr[i]][0]);
                links.RemoveAt(rmArr[i]);

            }

            dataVar[rm.Key.Item1][rm.Key.Item2].links = links.ToArray();
        }
    }

    void BeginDepenciesBuild() {

        nodeBranchingData = new int[dataVar.Length];
        autoManagedVariables = new int[dataVar.Length][];
        boolData = new AbilityBooleanData(dataVar);

        for(int i = 0; i < dataVar.Length; i++) {
            List<int> aMVar = new List<int>();
            //Debug.Log("Printing for Node: " + i);

            for(int j = 0; j < dataVar[i].Length; j++) {

                //Debug.Log("Printing for Variable: " + j);
                bool interchangeable = LoadedData.GetVariableType(dataType[i], j, VariableTypes.INTERCHANGEABLE);
                AutoPopulationList<List<int[]>> varLinks = new AutoPopulationList<List<int[]>>(1);

                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    //Debug.LogFormat("{0},{1}", currLink[0], currLink[1]);

                    bool signal = currLink[2] == (int)LinkMode.SIGNAL ? true : false;//LoadedData.GetVariableType(dataType[i], j, VariableTypes.SIGNAL_ONLY);
                    // Marks target as true so it will be blocked.
                    if(!signal)
                        if(dataVar[i][j].field.t == dataVar[currLink[0]][currLink[1]].field.t || interchangeable) {
                            boolData.varsBlocked[currLink[0]][currLink[1]] = true;
                            //Debug.LogFormat("From Node {0} Variable {1} link {2} name {3}", i, j, k, dataVar[i][j].field.n);
                            //Debug.LogFormat("To Node {0} Variable {1} link {2} signal {3} interchange {4} name {5}", currLink[0], currLink[1], k, signal, interchangeable, dataVar[i][j].field.n);
                            //Debug.Log("This was called true.");
                        }
                }

                if(LoadedData.GetVariableType(dataType[i], j, VariableTypes.BLOCKED))
                    boolData.varsBlocked[i][j] = true;

                if(LoadedData.GetVariableType(dataType[i], j, VariableTypes.AUTO_MANAGED))
                    aMVar.Add(j);

                if(!LoadedData.GetVariableType(dataType[i], j, VariableTypes.NON_LINK))
                    nodeBranchingData[i] += dataVar[i][j].links.Length;
            }
            autoManagedVariables[i] = aMVar.ToArray();
        }
    }

    public void InputCallback(int i) {
        AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool(ClientProgram.clientId);
        centralPool.AddVariableNetworkData(new AbilityNodeNetworkData<string>(-1, -1, abilityId));
        CreateAbility(centralPool);

        //if(ClientProgram.clientInst) {
        //AbilityNodeNetworkData[] data = centralPool.GetVariableNetworkData();

        //AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
        ///encoder.SendInputSignal(centralPool, abilityId, data);
        //}
    }

    public void CreateAbility(AbilityCentralThreadPool threadInst, int clusterId = -1) {

        int tId = AbilitiesManager.aData[ClientProgram.clientId].playerSpawnedCentrals.Add(threadInst);//AbilityCentralThreadPool.globalCentralList.Add(threadInst);

        //int nId = AbilityTreeNode.globalList.Add(new AbilityNodeHolder(tId.ToString(), a));
        Variable[][] clonedCopy = CloneRuntimeParams(dataVar);

        //Debug.Log(boolData.OutputValues());
        bool[][] clonedBoolValues = boolData.ReturnNewCopy();

        // Rather than create new instance, everything except variables will be taken from here.
        if(clusterId == -1)
            clusterId = AbilityCentralThreadPool.globalCentralClusterList.Add(new List<int>());

        AbilityCentralThreadPool.globalCentralClusterList.l[clusterId].Add(tId);
        threadInst.SetCentralData(ClientProgram.clientId,tId, clonedCopy, dataType, nodeBranchingData, clonedBoolValues, autoManagedVariables, clusterId,targettedNodes);
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
        public Dictionary<string, AbilityData> abilties;
        public Dictionary<string, Sprite> assetData;
        public Dictionary<int, string> abilityManifest;
        public AutoPopulationList<AbilityCentralThreadPool> playerSpawnedCentrals;

        public PlayerAssetData() {
            assetData = new Dictionary<string, Sprite>();
            playerSpawnedCentrals = new AutoPopulationList<AbilityCentralThreadPool>();           
        }
    }



    public static Dictionary<int, PlayerAssetData> aData;
    public SpawnerOutput abilities;

    void Start() {
        string priCharacterId = aData[ClientProgram.clientId].abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER];

        //AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
        aData[ClientProgram.clientId].abilties[priCharacterId].InputCallback(0);
        //encoder.SendInputSignal(priCharacterId,null);
        abilities = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));
        (abilities.script as LinearLayout).o = LinearLayout.Orientation.X;
        abilities.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.1f));
        AssignInputKeys();
    }

    public void AssignInputKeys() {
        if(aData.ContainsKey(ClientProgram.clientId))
            foreach(var ability in aData[ClientProgram.clientId].abilties)
                if(!aData[ClientProgram.clientId].abilityManifest.ContainsValue(ability.Key)) {
                    int keyAssigned = aData[ClientProgram.clientId].abilties[ability.Key].abilityInfo.kC;
                    LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[ClientProgram.clientId].abilties[ability.Key], 0, (KeyCode)keyAssigned, 0, true);

                    SpawnerOutput abilityButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
                    UIDrawer.GetTypeInElement<Text>(abilityButton).text = aData[ClientProgram.clientId].abilties[ability.Key].abilityInfo.n;
                    (abilities.script as LinearLayout).Add(abilityButton.script.transform as RectTransform);

                    UIDrawer.GetTypeInElement<Button>(abilityButton).onClick.AddListener(() => {
                        aData[ClientProgram.clientId].abilties[ability.Key].InputCallback(0);
                    });
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

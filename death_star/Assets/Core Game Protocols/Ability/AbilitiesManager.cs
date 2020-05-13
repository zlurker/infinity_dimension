using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine.UI;

public interface INodeNetworkPoint {

    void ModifyDataPacket(AbilityNodeNetworkData dataPacket);
    void ProcessDataPacket<T>(AbilityNodeNetworkData<T> dataPacket);
}

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
    Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>> targettedNodes;
    Type[] dataType;

    int[] nodeProgenitor;
    int[][] rootSubclasses;
    int[] nodeBranchingData;
    int[][] autoManagedVariables;
    int playerId;
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

    public void SetNodeProgenitor(int node, int progenitor) {
        nodeProgenitor[node] = progenitor;
    }

    public void AddTargettedNode(int a1, int a2, ON_VARIABLE_CATERGORY subCategory, int b1) {
        Tuple<int, int> id = Tuple.Create<int, int>(a1, a2);

        if(!targettedNodes.ContainsKey(a1))
            targettedNodes.Add(a1, new Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>());

        if(!targettedNodes[a1].ContainsKey(subCategory))
            targettedNodes[a1].Add(subCategory, new Dictionary<int, HashSet<int>>());

        if(!targettedNodes[a1][subCategory].ContainsKey(a2))
            targettedNodes[a1][subCategory].Add(a2, new HashSet<int>());

        if(!targettedNodes[a1][subCategory][a2].Contains(b1))
            targettedNodes[a1][subCategory][a2].Add(b1);
    }

    public AbilityData(AbilityDataSubclass[] data, AbilityInfo aD, int pId, string aId) {

        abilityInfo = aD;
        // Sorts variable out accordingly.
        dataVar = new Variable[data.Length + 1][];
        dataType = new Type[data.Length + 1];
        linkData = new LinkData[data.Length + 1];
        nodeProgenitor = new int[data.Length + 1];
        targettedNodes = new Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>>();

        for(int i = 0; i < data.Length; i++) {
            dataVar[i] = data[i].var;
            dataType[i] = data[i].classType;
            linkData[i] = new LinkData();
        }

        playerId = pId;
        abilityId = aId;

        RetrieveStartNodes();

        // Needs to be rectified to add in the This node.
        int startNode = dataVar.Length - 1;
        dataVar[startNode] = new Variable[] { new Variable(LoadedData.loadedParamInstances[typeof(ThreadSplitter)].runtimeParameters[0].rP), new Variable(LoadedData.loadedParamInstances[typeof(ThreadSplitter)].runtimeParameters[1].rP, rootSubclasses) };
        dataType[startNode] = typeof(ThreadSplitter);
        linkData[startNode] = new LinkData();

        RunNodeFlow(startNode, startNode);
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
        Debug.Log(rootSubclasses.Length);
    }

    void RunNodeFlow(int nextNode, int progenitor) {

        if(LoadedData.loadedNodeInstance[dataType[nextNode]] is INodeNetworkPoint)
            progenitor = nextNode;

        nodeProgenitor[nextNode] = progenitor;

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
                RunNodeFlow(currLink[0], progenitor);
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

                if(LoadedData.GetVariableType(dataType[i], j, VariableTypes.GLOBAL_VARIABLE)) {
                    string gVN = (dataVar[i][j].field as RuntimeParameters<string>).v;
                    if (!AbilitiesManager.GetAssetData(playerId).globalVariables.ContainsKey(gVN))
                        AbilitiesManager.GetAssetData(playerId).globalVariables.Add(gVN, null);
                }
            }
            autoManagedVariables[i] = aMVar.ToArray();
        }
    }

    public void InputCallback(int i) {
        AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool(ClientProgram.clientId);
        SignalCentralCreation(centralPool);
        CreateAbility(centralPool, ClientProgram.clientId);
    }

    public void SignalCentralCreation(AbilityCentralThreadPool central) {
        central.AddVariableNetworkData(new AbilityNodeNetworkData<int>(-1, -1, playerId));
        central.AddVariableNetworkData(new AbilityNodeNetworkData<string>(-1, -1, abilityId));
    }


    public void CreateAbility(AbilityCentralThreadPool threadInst, int pId, int givenPopulatedId = -1, int clusterId = -1) {

        if(givenPopulatedId > -1)
            AbilitiesManager.aData[pId].playerSpawnedCentrals.ModifyElementAt(givenPopulatedId, threadInst);
        else
            givenPopulatedId = AbilitiesManager.aData[pId].InsertSpawnedIntoFreeSpace(threadInst);//AbilityCentralThreadPool.globalCentralList.Add(threadInst);

        //int nId = AbilityTreeNode.globalList.Add(new AbilityNodeHolder(tId.ToString(), a));
        Variable[][] clonedCopy = CloneRuntimeParams(dataVar);

        //Debug.Log(boolData.OutputValues());
        bool[][] clonedBoolValues = boolData.ReturnNewCopy();

        // Rather than create new instance, everything except variables will be taken from here.
        if(clusterId == -1)
            clusterId = AbilityCentralThreadPool.globalCentralClusterList.Add(new List<int>());

        AbilityCentralThreadPool.globalCentralClusterList.l[clusterId].Add(givenPopulatedId);
        threadInst.SetCentralData(pId, givenPopulatedId, clonedCopy, dataType, nodeBranchingData, clonedBoolValues, autoManagedVariables, clusterId, targettedNodes, nodeProgenitor);
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
        private int playerId;
        public Dictionary<string, AbilityData> abilties;
        public Dictionary<string, Sprite> assetData;
        public Dictionary<int, string> abilityManifest;
        public AutoPopulationList<AbilityCentralThreadPool> playerSpawnedCentrals;
        public Dictionary<string, int[]> globalVariables;
        //public Dictionary<int, Dictionary<string, VariableInterfaces>> globalVariables;

        private List<int> internalFreeSpaceTracker;

        public PlayerAssetData(int pId) {
            playerId = pId;

            assetData = new Dictionary<string, Sprite>();
            playerSpawnedCentrals = new AutoPopulationList<AbilityCentralThreadPool>();
            internalFreeSpaceTracker = new List<int>();
            globalVariables = new Dictionary<string,int[]>();
            //globalVariables = new Dictionary<int, Dictionary<string, VariableInterfaces>>();
        }

        public void RemoveSpawn(int index) {
            playerSpawnedCentrals.ModifyElementAt(index, null);
            internalFreeSpaceTracker.Add(index);
        }

        public int InsertSpawnedIntoFreeSpace(AbilityCentralThreadPool inst) {
            int index = 0;

            if(internalFreeSpaceTracker.Count > 0) {
                index = internalFreeSpaceTracker[0];
                internalFreeSpaceTracker.RemoveAt(0);
            } else
                index = playerSpawnedCentrals.l.Count;

            playerSpawnedCentrals.ModifyElementAt(index, inst);
            return index;
        }

        public void BuildGlobalVariables() {
            AbilityDataSubclass[] globalVariableNodes = new AbilityDataSubclass[globalVariables.Count];
            string[] keys = globalVariables.Keys.ToArray();
            

            for (int i=0; i < keys.Length; i++) {
                int[] instanceAddress = new int[2];
                globalVariableNodes[i] = new AbilityDataSubclass(typeof(GlobalVariables));
                (globalVariableNodes[i].var[LoadedData.loadedParamInstances[typeof(GlobalVariables)].variableAddresses["Variable Name"]].field as RuntimeParameters<string>).v = keys[i];

                instanceAddress[0] = playerId;
                instanceAddress[1] = i;

                globalVariables[keys[i]] = instanceAddress;
            }

            AbilityData globalVarInst = new AbilityData(globalVariableNodes, new AbilityInfo(), playerId, "");
            abilties.Add("", globalVarInst);

            //AbilityCentralThreadPool central = new AbilityCentralThreadPool(playerId);
            //globalVarInst.CreateAbility(central, playerId);
            //Debug.Log(central.GetNode(3));

            //Debug.Log("BGV was runned.");
        }
    }


    public static Dictionary<int, PlayerAssetData> aData;
    public SpawnerOutput abilities;

    void Start() {
        aData[ClientProgram.clientId].abilties[""].InputCallback(0);

        string priCharacterId = aData[ClientProgram.clientId].abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER];
        aData[ClientProgram.clientId].abilties[priCharacterId].InputCallback(0);

        abilities = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));
        (abilities.script as LinearLayout).o = LinearLayout.Orientation.X;
        abilities.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.1f));
        AssignInputKeys();
    }



    public void BuildGlobalVariables() {

    }

    public void AssignInputKeys() {
        if(aData.ContainsKey(ClientProgram.clientId))
            foreach(var ability in aData[ClientProgram.clientId].abilties)
                if(!aData[ClientProgram.clientId].abilityManifest.ContainsValue(ability.Key)) {
                    int keyAssigned = aData[ClientProgram.clientId].abilties[ability.Key].abilityInfo.kC;
                    LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[ClientProgram.clientId].abilties[ability.Key], 0, (KeyCode)keyAssigned, 0, true);

                    SpawnerOutput abilityButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
                    LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(abilityButton, "Text").text = aData[ClientProgram.clientId].abilties[ability.Key].abilityInfo.n;
                    (abilities.script as LinearLayout).Add(abilityButton.script.transform as RectTransform);

                    LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(abilityButton).onClick.AddListener(() => {
                        aData[ClientProgram.clientId].abilties[ability.Key].InputCallback(0);
                    });
                }
    }

    public static PlayerAssetData GetAssetData(int playerid) {
        if(aData.ContainsKey(playerid))
            return aData[playerid];

        PlayerAssetData inst = new PlayerAssetData(playerid);
        aData.Add(playerid, inst);
        return inst;
    }
}

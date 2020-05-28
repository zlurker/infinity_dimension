using Newtonsoft.Json;
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
    Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>> targettedNodes;
    Dictionary<int, int[]> nodeNetworkVariables;

    HashSet<Tuple<int, int, int>>[][][] linkGenerator;

    int[][][][][] generatedLinks;
    Type[] dataType;

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
        dataVar = new Variable[data.Length + 2][];
        dataType = new Type[data.Length + 2];
        linkData = new LinkData[data.Length + 2];
        targettedNodes = new Dictionary<int, Dictionary<ON_VARIABLE_CATERGORY, Dictionary<int, HashSet<int>>>>();
        nodeNetworkVariables = new Dictionary<int, int[]>();

        linkGenerator = new HashSet<Tuple<int, int, int>>[2][][];
        linkGenerator[0] = new HashSet<Tuple<int, int, int>>[data.Length + 2][];
        linkGenerator[1] = new HashSet<Tuple<int, int, int>>[data.Length + 2][];

        for(int i = 0; i < data.Length; i++) {
            dataVar[i] = data[i].var;

            linkGenerator[0][i] = new HashSet<Tuple<int, int, int>>[data[i].var.Length];
            linkGenerator[1][i] = new HashSet<Tuple<int, int, int>>[data[i].var.Length];

            dataType[i] = data[i].classType;
            linkData[i] = new LinkData();
        }

        playerId = pId;
        abilityId = aId;

        RetrieveStartNodes();

        int startNode = dataVar.Length - 2;
        dataVar[startNode] = new Variable[] { new Variable(LoadedData.loadedParamInstances[typeof(NodeThreadStarter)].runtimeParameters[0].rP.ReturnNewRuntimeParamCopy(), rootSubclasses) };
        dataType[startNode] = typeof(NodeThreadStarter);
        linkData[startNode] = new LinkData();
        linkGenerator[0][startNode] = new HashSet<Tuple<int, int, int>>[1];
        linkGenerator[1][startNode] = new HashSet<Tuple<int, int, int>>[1];

        int endNode = dataVar.Length - 1;
        dataVar[endNode] = new Variable[] { new Variable(LoadedData.loadedParamInstances[typeof(NodeThreadEndPoint)].runtimeParameters[0].rP.ReturnNewRuntimeParamCopy()) };
        dataType[endNode] = typeof(NodeThreadEndPoint);
        linkData[endNode] = new LinkData();
        linkGenerator[0][endNode] = new HashSet<Tuple<int, int, int>>[1];
        linkGenerator[1][endNode] = new HashSet<Tuple<int, int, int>>[1];

        RunNodeFlow(startNode, 0);
        GenerateLinks();

        EditLinks();
        BeginDepenciesBuild();
    }

    void RetrieveStartNodes() {
        int nonPsuedoNodes = dataVar.Length - 2;

        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(nonPsuedoNodes);

        for(int i = 0; i < nonPsuedoNodes; i++) {
            int totalCurrLinks = 0;

            for(int j = 0; j < dataVar[i].Length; j++) {
                totalCurrLinks += dataVar[i][j].links.Length;

                for(int k = 0; k < dataVar[i][j].links.Length; k++) {
                    int[] currLink = dataVar[i][j].links[k];

                    // Marks target as true so it can't be root.
                    connected.ModifyElementAt(currLink[0], true);
                }
            }

            if(totalCurrLinks == 0)
                dataVar[i][dataVar[i].Length - 1].links = new int[][] { new int[] { dataVar.Length - 1, 0, 0 } };
        }

        List<int[]> rC = new List<int[]>();

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rC.Add(new int[] { i, 0, 1 });

        rootSubclasses = rC.ToArray();
    }

    void RunNodeFlow(int nextNode, int targetVar, Tuple<int, int, int>[] pN = null) {

        int[] lC = LoadedData.loadedNodeInstance[dataType[nextNode]].ReturnLinkChannels();

        for(int k = 0; k < lC.Length; k++)
            if(pN != null) {
                Tuple<int, int, int> precedingNode = pN[lC[k]];

                if(linkGenerator[lC[k]][precedingNode.Item1][precedingNode.Item2] == null)
                    linkGenerator[lC[k]][precedingNode.Item1][precedingNode.Item2] = new HashSet<Tuple<int, int, int>>();

                Tuple<int, int, int> currNode = Tuple.Create(nextNode, targetVar, precedingNode.Item3);

                if(!linkGenerator[lC[k]][precedingNode.Item1][precedingNode.Item2].Contains(currNode))
                    linkGenerator[lC[k]][precedingNode.Item1][precedingNode.Item2].Add(currNode);
            }


        //if(LoadedData.loadedNodeInstance[dataType[nextNode]] is INodeNetworkPoint)
        //progenitor = nextNode;
        //Debug.LogFormat("Curr Node: {0}, {1}", nextNode,dataType[nextNode]);
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


                Tuple<int, int, int> nextNodeRef = Tuple.Create<int, int, int>(nextNode, i, currLink[2]);
                List<Tuple<int, int, int>> precedingArray = new List<Tuple<int, int, int>>();

                if(pN != null)
                    precedingArray.AddRange(pN);
                else {
                    precedingArray.Add(null);
                    precedingArray.Add(null);
                }

                for(int k = 0; k < lC.Length; k++)
                    precedingArray[lC[k]] = nextNodeRef;

                // Iterates to target.
                RunNodeFlow(currLink[0], currLink[1], precedingArray.ToArray());
            }

        //Debug.LogWarningFormat("End of Node: {0}, {1}", nextNode, dataType[nextNode]);
    }

    void GenerateLinks() {
        // Generate links after
        generatedLinks = new int[linkGenerator.Length][][][][];

        for(int i = 0; i < linkGenerator.Length; i++) {
            generatedLinks[i] = new int[linkGenerator[i].Length][][][];

            for(int j = 0; j < linkGenerator[i].Length; j++) {
                generatedLinks[i][j] = new int[linkGenerator[i][j].Length][][];

                for(int k = 0; k < linkGenerator[i][j].Length; k++) {
                    List<int[]> convertedLinks = new List<int[]>();
                    //generatedLinks[i][j][k] = new int[linkGenerator[i][j][k].Count];

                    if(linkGenerator[i][j][k] != null)
                        foreach(var item in linkGenerator[i][j][k])
                            convertedLinks.Add(new int[] { item.Item1, item.Item2, item.Item3 });


                    //Debug.LogFormat("Generating for: {0}, {1}, {2}", i, j, k);
                    generatedLinks[i][j][k] = convertedLinks.ToArray();
                }
            }
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
            List<int> networkVariables = new List<int>();
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
                    if(!AbilitiesManager.GetAssetData(playerId).globalVariables.ContainsKey(gVN))
                        AbilitiesManager.GetAssetData(playerId).globalVariables.Add(gVN, null);
                }

                if(LoadedData.GetVariableType(dataType[i], j, VariableTypes.NETWORK))
                    networkVariables.Add(j);
            }

            autoManagedVariables[i] = aMVar.ToArray();

            if(networkVariables.Count > 0)
                nodeNetworkVariables.Add(i, networkVariables.ToArray());
            //aMVar.Add(j);

        }
    }

    public void InputCallback(int i) {
        //AbilitiesManager.aData[playerId].abilties[]
        AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool(playerId);
        SignalCentralCreation(centralPool);
        CreateAbility(centralPool, ClientProgram.clientId);

        if(ClientProgram.hostId == ClientProgram.clientId)
            centralPool.StartThreads(0);
        else
            centralPool.StartThreads(1);
        //(NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.INPUT_SIGNAL] as InputSignalEncoder).SendInputSignal(playerId, abilityId);        
    }



    public void SignalCentralCreation(AbilityCentralThreadPool central) {
        central.AddVariableNetworkData(new AbilityNodeNetworkData<int>(-1, -1, playerId));
        central.AddVariableNetworkData(new AbilityNodeNetworkData<string>(-1, -1, abilityId));
    }

    /*public void CreateAbilityNetworkData() {
        AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool(playerId);
        SignalCentralCreation(centralPool);
        CreateAbility(centralPool, ClientProgram.clientId);
    }*/


    public void CreateAbility(AbilityCentralThreadPool threadInst, int pId, int givenPopulatedId = -1) {

        if(givenPopulatedId > -1)
            AbilitiesManager.aData[pId].playerSpawnedCentrals.ModifyElementAt(givenPopulatedId, threadInst);
        else
            givenPopulatedId = AbilitiesManager.aData[pId].InsertSpawnedIntoFreeSpace(threadInst);//AbilityCentralThreadPool.globalCentralList.Add(threadInst);

        //int nId = AbilityTreeNode.globalList.Add(new AbilityNodeHolder(tId.ToString(), a));
        //Variable[][] clonedCopy = CloneRuntimeParams(dataVar);

        RuntimeParameters[][] clonedRp = new RuntimeParameters[dataVar.Length][];
        int[][][][][] linkMap = new int[1][][][][];
        linkMap[0] = new int[dataVar.Length][][][];

        for(int i = 0; i < dataVar.Length; i++) {
            clonedRp[i] = new RuntimeParameters[dataVar[i].Length];
            linkMap[0][i] = new int[dataVar[i].Length][][];

            for(int j = 0; j < dataVar[i].Length; j++) {
                clonedRp[i][j] = dataVar[i][j].field.ReturnNewRuntimeParamCopy();
                linkMap[0][i][j] = dataVar[i][j].links;
            }
        }


        //Debug.Log(boolData.OutputValues());
        bool[][] clonedBoolValues = boolData.ReturnNewCopy();

        threadInst.SetCentralData(pId, givenPopulatedId, clonedRp, generatedLinks, dataType, nodeBranchingData, clonedBoolValues, autoManagedVariables, targettedNodes, nodeNetworkVariables);

        //if(startThreads)
        //    threadInst.StartThreads();
        //threadInst.SendVariableNetworkData();
    }

    /*Variable[][] CloneRuntimeParams(Variable[][] target) {
        Variable[][] clonedCopy = new Variable[target.Length][];

        for(int i = 0; i < target.Length; i++) {

            clonedCopy[i] = new Variable[target[i].Length];

            for(int j = 0; j < target[i].Length; j++)
                clonedCopy[i][j] = new Variable(target[i][j].field.ReturnNewRuntimeParamCopy(), target[i][j].links);
        }
        return clonedCopy;
    }*/
}

public sealed class AbilitiesManager : MonoBehaviour {

    public class PlayerAssetData {
        private int playerId;
        public Dictionary<string, AbilityData> abilties;
        public Dictionary<string, Sprite> assetData;
        public Dictionary<int, string> abilityManifest;
        public AutoPopulationList<AbilityCentralThreadPool> playerSpawnedCentrals;
        public Dictionary<string, Tuple<int, int, int>> globalVariables;
        //public Dictionary<int, Dictionary<string, VariableInterfaces>> globalVariables;

        private List<int> internalFreeSpaceTracker;

        public PlayerAssetData(int pId) {
            playerId = pId;

            assetData = new Dictionary<string, Sprite>();
            playerSpawnedCentrals = new AutoPopulationList<AbilityCentralThreadPool>();
            internalFreeSpaceTracker = new List<int>();
            globalVariables = new Dictionary<string, Tuple<int, int, int>>();
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


            for(int i = 0; i < keys.Length; i++) {
                int[] instanceAddress = new int[2];
                globalVariableNodes[i] = new AbilityDataSubclass(typeof(GlobalVariables));
                (globalVariableNodes[i].var[LoadedData.loadedParamInstances[typeof(GlobalVariables)].variableAddresses["Variable Name"]].field as RuntimeParameters<string>).v = keys[i];

                globalVariables[keys[i]] = Tuple.Create(playerId, 0, i);

                //Debug.LogWarning("Create Node ID: " + keys[i] + " " + globalVariables[keys[i]]);
            }

            AbilityData globalVarInst = new AbilityData(globalVariableNodes, new AbilityInfo(), playerId, "");
            abilties.Add("", globalVarInst);
        }
    }

    public static bool playerLoadedInLobby;
    public static List<byte[]> pendingData = new List<byte[]>();

    public static Dictionary<int, PlayerAssetData> aData;
    public SpawnerOutput abilities;

    void Start() {

        // Applies pending data that was recieved while loading.
        Debug.Log("Updating " + pendingData.Count + " messages while loading.");
        for(int i = 0; i < pendingData.Count; i++)
            (NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.UPDATE_ABILITY_DATA] as UpdateAbilityDataEncoder).ParseMessage(pendingData[i]);

        pendingData.Clear();
        playerLoadedInLobby = true;

        // Creates global variables for this player.
        aData[ClientProgram.clientId].abilties[""].InputCallback(0);

        // Creates player main character.
        string priCharacterId = aData[ClientProgram.clientId].abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER];
        aData[ClientProgram.clientId].abilties[priCharacterId].InputCallback(0);

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

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public sealed class AbilitiesManager : MonoBehaviour {

    public class PlayerAssetData {
        public AbilityData[] abilties;
        public Dictionary<string, Sprite> assetData;

        public PlayerAssetData() {
            assetData = new Dictionary<string, Sprite>();
        }
    }

    public class AbilityData : IInputCallback<int> {
        // Data that needs to be read/written
        Variable[][] dataVar;
        AbilityBooleanData boolData;

        // Data that will purely only be read.
        string[] description;
        Type[] dataType;
        int[] rootSubclasses;
        int[] nodeType;
        int[] nodeBranchingData;
        Dictionary<int, int> specialisedNodeData;
        int abilityId;

        public AbilityData(Variable[][] dV, Type[] dT, int[] rS, int[] nT, int[] nBD, Dictionary<int, int> sND, int aId, AbilityBooleanData aBD) {
            dataVar = dV;
            dataType = dT;
            rootSubclasses = rS;
            nodeType = nT;
            nodeBranchingData = nBD;
            specialisedNodeData = sND;
            abilityId = aId;
            boolData = aBD;
        }

        public void InputCallback(int i) {
            SyncInputWithNetwork();
        }

        public void SyncInputWithNetwork() {
            if(ClientProgram.clientInst) {
                AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
                encoder.SendInputSignal(abilityId);
            } else {
                AbilityCentralThreadPool centralPool = new AbilityCentralThreadPool();
                CreateAbility(centralPool);
            }
        }

        public void CreateAbility(AbilityCentralThreadPool threadInst) {

            int tId = AbilityCentralThreadPool.globalCentralList.Add(threadInst);

            AbilityTreeNode[] a = new AbilityTreeNode[dataVar.Length];
            int nId = AbilityTreeNode.globalList.Add(new AbilityNodeHolder(tId.ToString(), a));
            Variable[][] clonedCopy = CloneRuntimeParams(dataVar);
            bool[][] clonedBoolValues = boolData.ReturnNewCopy();

            // Rather than create new instance, everything except variables will be taken from here.
            threadInst.SetCentralData(tId, nId, clonedCopy, dataType, rootSubclasses, nodeBranchingData, nodeType, specialisedNodeData, clonedBoolValues);
            threadInst.StartThreads();
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

    public static Dictionary<int, PlayerAssetData> aData;
    
    void Start() {
        AssignInputKeys();        
    }

    public void AssignInputKeys() {
        for (int i=0; i< aData[ClientProgram.clientId].abilties.Length; i++) 
            LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[ClientProgram.clientId].abilties[i], 0, (KeyCode)97 + i, 0);       
    }

    public static PlayerAssetData GetAssetData(int playerid) {
        if(aData.ContainsKey(playerid))
            return aData[playerid];

        PlayerAssetData inst = new PlayerAssetData();
        aData.Add(playerid, inst);
        return inst;
    }

    void LoadArtAssets() {
        /*assetData = new Dictionary<string, Sprite>();

        string[] imagePaths = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].LoadAllDir(0);
        
        for(int i = 0; i < imagePaths.Length; i++) {

            Texture2D generatedTex = new Texture2D(1, 1);
            generatedTex.LoadImage(File.ReadAllBytes(imagePaths[i]));

            Sprite sprInst = Sprite.Create(generatedTex, new Rect(0, 0, generatedTex.width, generatedTex.height), new Vector2(0.5f, 0.5f));

            assetData.Add(Path.GetFileName(imagePaths[i]), sprInst);
        }*/
    }

    void LoadAbilityData() {
        /*string[] abilityNodeData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(0);
        string[] abilityRootData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(3);
        string[] abilityNodeBranchingData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(4);
        string[] abilitySpecialisedData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(5);
        string[] variableBlockData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadAll(6);

        aData = new AbilityData[abilityNodeData.Length];

        for(int i = 0; i < abilityNodeData.Length; i++) {

            AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData[i]));

            if(ability == null)
                continue;

            int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData[i]);
            int[] nodeBranchData = JsonConvert.DeserializeObject<int[]>(abilityNodeBranchingData[i]);
            Dictionary<int, int> specialisedNodeData = JsonConvert.DeserializeObject<Dictionary<int, int>>(abilitySpecialisedData[i]);
            AbilityBooleanData boolData = JsonConvert.DeserializeObject<AbilityBooleanData>(variableBlockData[i]);

            Variable[][] tempVar = new Variable[ability.Length][];
            Type[] tempTypes = new Type[ability.Length];

            for(int j = 0; j < ability.Length; j++) {
                tempVar[j] = ability[j].var;
                tempTypes[j] = ability[j].classType;
            }

            int[] nodeType = new int[ability.Length];

            aData[i] = new AbilityData(tempVar, tempTypes, rootSubclasses, nodeType, nodeBranchData, specialisedNodeData, i, boolData);
            LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[i], 0, (KeyCode)97 + i, 0);
        }*/
    }
}

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
        public Dictionary<int, int> abilityManifest;

        public PlayerAssetData() {
            assetData = new Dictionary<string, Sprite>();
            abilityManifest = new Dictionary<int, int>();
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
        int priCharacterId = aData[ClientProgram.clientId].abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER];

        AbilityInputEncoder encoder = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.ABILITY_INPUT] as AbilityInputEncoder;
        encoder.SendInputSignal(priCharacterId);
        AssignInputKeys();
    }

    public void AssignInputKeys() {
        if(aData.ContainsKey(ClientProgram.clientId)) {
            int actualKeyLoaded = 0;
            for(int i = 0; i < aData[ClientProgram.clientId].abilties.Length; i++)
                if(!aData[ClientProgram.clientId].abilityManifest.ContainsValue(i)) {
                    LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[ClientProgram.clientId].abilties[i], 0, (KeyCode)(97 + actualKeyLoaded), 0,true);
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

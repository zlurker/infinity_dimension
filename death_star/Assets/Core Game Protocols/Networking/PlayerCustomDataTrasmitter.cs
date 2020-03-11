﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    static int[] datafilesToSend = new int[] { 0, 1, 3, 4, 5, 6 };
    public Dictionary<int, List<string>> builders;

    public int expectedFiles;
    public int sentFiles;

    public void ResetTransmitter() {
        builders = new Dictionary<int, List<string>>();
        expectedFiles = 0;
        sentFiles = 0;
    }

    public void SendFiles() {

        byte[][][] cData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(datafilesToSend);

        expectedFiles = cData.Length * datafilesToSend.Length;
        SetBytesToSend(BitConverter.GetBytes(cData.Length));

        for(int i = 0; i < cData.Length; i++)
            for(int j = 0; j < cData[i].Length; j++)
                SetBytesToSend(cData[i][j]);
    }

    public override void MessageRecievedCallback() {
         Debug.Log("Incoming data.");

        if(!builders.ContainsKey(targetId)) {
            int recvSize = BitConverter.ToInt32(bytesRecieved, 0);
            builders.Add(targetId, new List<string>());

            AbilitiesManager.GetAssetData(targetId).abilties = new AbilitiesManager.AbilityData[recvSize];
            //AbilitiesManager.aData.Add(targetId, new AbilitiesManager.AbilityData[recvSize]);
            return;
        }

        string fileContent = Encoding.Default.GetString(bytesRecieved);
        builders[targetId].Add(fileContent);

        if(targetId == ClientProgram.clientId)
            sentFiles++;

        if(builders[targetId].Count % datafilesToSend.Length == 0)
            BuildAbility(targetId);
    }

    public void BuildAbility(int targetId) {

        int latestEntry = builders[targetId].Count - 1;

        string abilityNodeData = builders[targetId][latestEntry - 5];
        string abilityDescription = builders[targetId][latestEntry - 4];
        string abilityRootData = builders[targetId][latestEntry - 3];
        string abilityNodeBranchingData = builders[targetId][latestEntry - 2];
        string abilitySpecialisedData = builders[targetId][latestEntry - 1];
        string variableBlockData = builders[targetId][latestEntry];

        AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData));

        if(ability == null)
            return;

        int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData);
        int[] nodeBranchData = JsonConvert.DeserializeObject<int[]>(abilityNodeBranchingData);
        Dictionary<int, int> specialisedNodeData = JsonConvert.DeserializeObject<Dictionary<int, int>>(abilitySpecialisedData);
        AbilityBooleanData boolData = JsonConvert.DeserializeObject<AbilityBooleanData>(variableBlockData);

        Variable[][] tempVar = new Variable[ability.Length][];
        Type[] tempTypes = new Type[ability.Length];

        for(int j = 0; j < ability.Length; j++) {
            tempVar[j] = ability[j].var;
            tempTypes[j] = ability[j].classType;
        }

        int[] nodeType = new int[ability.Length];

        int currAbility = builders[targetId].Count / datafilesToSend.Length;
        currAbility--;

        AbilitiesManager.aData[targetId].abilties[currAbility] = new AbilitiesManager.AbilityData(tempVar, tempTypes, rootSubclasses, nodeType, nodeBranchData, specialisedNodeData, currAbility, boolData);
        //aData[i] = 
        //LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[i], 0, (KeyCode)97 + i, 0);
    }
}
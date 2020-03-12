using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    static int[] datafilesToSend = new int[] { 0, 1, 3, 4, 5, 6 };
    public Dictionary<int, List<byte[]>> builders;

    public int expectedFiles;
    public int sentFiles;

    public void ResetTransmitter() {
        builders = new Dictionary<int, List<byte[]>>();
        expectedFiles = 0;
        sentFiles = 0;
    }

    public void SendFiles() {

        Dictionary<string, byte[][]> directoryBytesData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(datafilesToSend);
        Dictionary<int, string> abilityManifest = null;
        HashSet<string> manifestFiles = new HashSet<string>();

        string abilityManifestPath = Path.Combine(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP, "AbilityManifest.json");

        expectedFiles = (directoryBytesData.Count * datafilesToSend.Length) + directoryBytesData.Count;
        SetBytesToSend(BitConverter.GetBytes(directoryBytesData.Count));
        int givenCategory = -1;

        if(File.Exists(abilityManifestPath)) {
            string fileContents = File.ReadAllText(abilityManifestPath);
            abilityManifest = JsonConvert.DeserializeObject<Dictionary<int, string>>(fileContents);

            foreach(var element in abilityManifest) {
                if(!manifestFiles.Contains(element.Value))
                    manifestFiles.Add(element.Value);

                int manifestCate = element.Key;
                SetBytesToSend(BitConverter.GetBytes(manifestCate));

                for(int i = 0; i < directoryBytesData[element.Value].Length; i++)
                    SetBytesToSend(directoryBytesData[element.Value][i]);
            }
        }

        foreach(var element in directoryBytesData) {

            if(!manifestFiles.Contains(element.Key)) {
                SetBytesToSend(BitConverter.GetBytes(givenCategory));

                for(int i = 0; i < element.Value.Length; i++)
                    SetBytesToSend(element.Value[i]);
            }
        }
    }

    public override void MessageRecievedCallback() {
        Debug.Log("Incoming data.");

        if(!builders.ContainsKey(targetId)) {
            int recvSize = BitConverter.ToInt32(bytesRecieved, 0);
            builders.Add(targetId, new List<byte[]>());

            AbilitiesManager.GetAssetData(targetId).abilties = new AbilitiesManager.AbilityData[recvSize];
            return;
        }

        builders[targetId].Add(bytesRecieved);

        if(targetId == ClientProgram.clientId)
            sentFiles++;

        if(builders[targetId].Count % (datafilesToSend.Length + 1) == 0)
            BuildAbility(targetId);
    }

    public void BuildAbility(int targetId) {

        int latestEntry = builders[targetId].Count - 1;

        int catergory = BitConverter.ToInt32(builders[targetId][latestEntry - 6], 0);

        Debug.Log("Catergory: " + catergory);

        string abilityNodeData = Encoding.Default.GetString(builders[targetId][latestEntry - 5]);
        string abilityDescription = Encoding.Default.GetString(builders[targetId][latestEntry - 4]);
        string abilityRootData = Encoding.Default.GetString(builders[targetId][latestEntry - 3]);
        string abilityNodeBranchingData = Encoding.Default.GetString(builders[targetId][latestEntry - 2]);
        string abilitySpecialisedData = Encoding.Default.GetString(builders[targetId][latestEntry - 1]);
        string variableBlockData = Encoding.Default.GetString(builders[targetId][latestEntry]);

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

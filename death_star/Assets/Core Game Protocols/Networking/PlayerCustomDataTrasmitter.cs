using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    static int[] datafilesToSend = new int[] { 0, 1, 3, 4, 5, 6, 8 };
    public Dictionary<int, List<byte[]>> builders;

    public int expectedFiles;
    public int sentFiles;

    public void ResetTransmitter() {
        builders = new Dictionary<int, List<byte[]>>();
        expectedFiles = 0;
        sentFiles = 0;
    }

    public void SendFiles() {

        DirectoryBytesData dData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(datafilesToSend);

        HashSet<string> manifestFiles = new HashSet<string>();
        Dictionary<string, int> remappedFiles = new Dictionary<string, int>();

        expectedFiles += dData.filesData.Length * datafilesToSend.Length;
        SetBytesToSend(BitConverter.GetBytes(dData.filesData.Length));

        for(int i = 0; i < dData.filesData.Length; i++) {

            remappedFiles.Add(dData.dirName[i], i);

            for(int j = 0; j < dData.filesData[i].Length; j++)
                SetBytesToSend(dData.filesData[i][j]);
        }

        ManifestEncoder mEncoder = encoders[(int)NetworkEncoderTypes.MANIFEST] as ManifestEncoder;
        mEncoder.SendManifest(remappedFiles);
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

        if(builders[targetId].Count % datafilesToSend.Length == 0)
            BuildAbility(targetId);
    }

    public void BuildAbility(int targetId) {

        int latestEntry = builders[targetId].Count - 1;

        string abilityNodeData = Encoding.Default.GetString(builders[targetId][latestEntry - 6]);
        string abilityDescription = Encoding.Default.GetString(builders[targetId][latestEntry - 5]);
        string abilityRootData = Encoding.Default.GetString(builders[targetId][latestEntry - 4]);
        string abilityNodeBranchingData = Encoding.Default.GetString(builders[targetId][latestEntry - 3]);
        string abilitySpecialisedData = Encoding.Default.GetString(builders[targetId][latestEntry - 2]);
        string variableBlockData = Encoding.Default.GetString(builders[targetId][latestEntry - 1]);
        string gDataStr = Encoding.Default.GetString(builders[targetId][latestEntry]);

        AbilityDataSubclass[] ability = JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData));

        if(ability == null)
            return;

        int[] rootSubclasses = JsonConvert.DeserializeObject<int[]>(abilityRootData);
        int[] nodeBranchData = JsonConvert.DeserializeObject<int[]>(abilityNodeBranchingData);
        Dictionary<int, int> specialisedNodeData = JsonConvert.DeserializeObject<Dictionary<int, int>>(abilitySpecialisedData);
        Dictionary<int, int[][]> gData = new Dictionary<int, int[][]>();
        AbilityBooleanData boolData = JsonConvert.DeserializeObject<AbilityBooleanData>(variableBlockData);
        Dictionary<Tuple<int, int>, int[]> getData = JsonConvert.DeserializeObject< Dictionary<Tuple<int, int>, int[]>>(gDataStr);

        Dictionary<Tuple<int, int>, int[][]> reorgGetData = new Dictionary<Tuple<int, int>, int[][]>();
        Variable[][] tempVar = new Variable[ability.Length][];
        Type[] tempTypes = new Type[ability.Length];   
        
        foreach (var kP in getData) {           
            List<int[]> tLinks = new List<int[]>(ability[kP.Key.Item1].var[kP.Key.Item2].links);

            int[][] replacedLinks = new int[kP.Value.Length][];

            for(int i = kP.Value.Length-1; i >=0 ; i--) {
                replacedLinks[i] = ability[kP.Key.Item1].var[kP.Key.Item2].links[kP.Value[i]];
                tLinks.RemoveAt(i);
            }

            // Does the data swap.
            reorgGetData.Add(kP.Key, tLinks.ToArray());
            ability[kP.Key.Item1].var[kP.Key.Item2].links = replacedLinks;
        }

        for(int i = 0; i < ability.Length; i++) {
            tempVar[i] = ability[i].var;
            tempTypes[i] = ability[i].classType;
        }

        int[] nodeType = new int[ability.Length];

        int currAbility = builders[targetId].Count / datafilesToSend.Length;
        currAbility--;

        AbilitiesManager.aData[targetId].abilties[currAbility] = new AbilitiesManager.AbilityData(tempVar, tempTypes, rootSubclasses, nodeBranchData, specialisedNodeData, currAbility, boolData,reorgGetData);
        //aData[i] = 
        //LoadedData.GetSingleton<PlayerInput>().AddNewInput(aData[i], 0, (KeyCode)97 + i, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    static int[] datafilesToSend = new int[] { 0, 1};
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

            AbilitiesManager.GetAssetData(targetId).abilties = new AbilityData[recvSize];
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

        string abilityNodeData = Encoding.Default.GetString(builders[targetId][latestEntry - 1]);
        string abilityDescription = Encoding.Default.GetString(builders[targetId][latestEntry]);

        AbilityDataSubclass[] ability = LoadedData.GetSingleton<JSONFileConvertor>().ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData));

        if(ability == null)
            return;

        int currAbility = builders[targetId].Count / datafilesToSend.Length;
        currAbility--;

        AbilitiesManager.aData[targetId].abilties[currAbility] = new AbilityData(ability,currAbility);
    }
}

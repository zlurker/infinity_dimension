using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;

public class PlayerCustomDataTrasmitter : NetworkMessageEncoder {

    public class AbilityBuilder {
        public List<byte[]> byteData;
        public int expectedFiles;

        public AbilityBuilder(int eF) {
            expectedFiles = eF;
            byteData = new List<byte[]>();
        }
    }

    static int[] datafilesToSend = new int[] { 0, 1};
    int additionalDataOffset = 1;
    public Dictionary<int, AbilityBuilder> builders;

    public int expectedFiles;
    public int sentFiles;

    public void ResetTransmitter() {
        builders = new Dictionary<int, AbilityBuilder>();
        expectedFiles = 0;
        sentFiles = 0;
    }

    public void SendFiles() {

        DirectoryBytesData dData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(datafilesToSend);

        HashSet<string> manifestFiles = new HashSet<string>();

        expectedFiles += dData.filesData.Length * GetDataBundleLength();
        SetBytesToSend(BitConverter.GetBytes(dData.filesData.Length));

        for(int i = 0; i < dData.filesData.Length; i++) {

            SetBytesToSend(Encoding.Default.GetBytes(dData.dirName[i]));

            for(int j = 0; j < dData.filesData[i].Length; j++)
                SetBytesToSend(dData.filesData[i][j]);
        }

        ManifestEncoder mEncoder = encoders[(int)NetworkEncoderTypes.MANIFEST] as ManifestEncoder;
        mEncoder.SendManifest();
    }

    public int GetDataBundleLength() {
        return datafilesToSend.Length + additionalDataOffset;
    }

    public override void MessageRecievedCallback() {
        //Debug.Log("Incoming data.");

        if(!builders.ContainsKey(targetId)) {
            int incomingAbilityDatasets = BitConverter.ToInt32(bytesRecieved, 0);
            Debug.Log("incomingAbilityDatasets " + incomingAbilityDatasets);
            builders.Add(targetId, new AbilityBuilder(incomingAbilityDatasets));

            AbilitiesManager.GetAssetData(targetId).abilties = new Dictionary<string, AbilityData>();
            return;
        }

        builders[targetId].byteData.Add(bytesRecieved);

        if(targetId == ClientProgram.clientId)
            sentFiles++;

        
        if(builders[targetId].byteData.Count % GetDataBundleLength() == 0) {

            BuildAbility(targetId);

            if(builders[targetId].byteData.Count / GetDataBundleLength() == builders[targetId].expectedFiles)
                AbilitiesManager.GetAssetData(targetId).BuildGlobalVariables();

        }
    }

    public void BuildAbility(int targetId) {

        int latestEntry = builders[targetId].byteData.Count - 1;

        string abilityId = Encoding.Default.GetString(builders[targetId].byteData[latestEntry - 2]);
        string abilityNodeData = Encoding.Default.GetString(builders[targetId].byteData[latestEntry - 1]);
        string abilityDescription = Encoding.Default.GetString(builders[targetId].byteData[latestEntry]);

        AbilityDataSubclass[] ability = LoadedData.GetSingleton<JSONFileConvertor>().ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(abilityNodeData));

        if(ability == null)
            return;

        AbilityInfo aD = JsonConvert.DeserializeObject<AbilityInfo>(abilityDescription);

        AbilitiesManager.aData[targetId].abilties[abilityId] = new AbilityData(ability,aD,targetId,abilityId);
    }
}

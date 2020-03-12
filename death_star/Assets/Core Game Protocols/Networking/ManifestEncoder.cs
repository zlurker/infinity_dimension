using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class ManifestEncoder : NetworkMessageEncoder {

    public Dictionary<int, List<int>> manifestBuilder;
    public int expectedDataCount;
    public int sentData;

    public void ResetManifestEncoder() {
        expectedDataCount = 0;
        sentData = 0;
        manifestBuilder = new Dictionary<int, List<int>>();
    }

    public override void MessageRecievedCallback() {
        if(targetId == ClientProgram.clientId) 
            sentData++;

        int manifestData = BitConverter.ToInt32(bytesRecieved,0);

        if(!manifestBuilder.ContainsKey(targetId))
            manifestBuilder.Add(targetId, new List<int>());

        manifestBuilder[targetId].Add(manifestData);
        
        if(manifestBuilder[targetId].Count % 2 == 0) {
            int currDataCount = manifestBuilder[targetId].Count - 1;

            int key = manifestBuilder[targetId][currDataCount - 1];
            int value = manifestBuilder[targetId][currDataCount];
            AbilitiesManager.GetAssetData(targetId).abilityManifest.Add(key,value);
        }        
    }

    public void SendManifest(Dictionary<string,int> remappedId) {

        Dictionary<int, string> abilityManifest = null;
        string abilityManifestPath = Path.Combine(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP, "AbilityManifest.json");

        if(File.Exists(abilityManifestPath)) {
            string fileContents = File.ReadAllText(abilityManifestPath);
            abilityManifest = JsonConvert.DeserializeObject<Dictionary<int, string>>(fileContents);

            Dictionary<int, int> reformatManifest = new Dictionary<int, int>();

            foreach(var element in abilityManifest)
                reformatManifest.Add(element.Key, remappedId[element.Value]);

            expectedDataCount = reformatManifest.Count *2;

            foreach(var element in reformatManifest) {
                SetBytesToSend(BitConverter.GetBytes(element.Key));
                SetBytesToSend(BitConverter.GetBytes(element.Value));
            }
        }        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Text;

public class ManifestEncoder : NetworkMessageEncoder {

    public int expectedDataCount;
    public int sentData;

    public void ResetManifestEncoder() {
        sentData = 0;
    }

    public override void MessageRecievedCallback() {
        if(targetId == ClientProgram.clientId) 
            sentData++;

        string jsonManifest = Encoding.Default.GetString(bytesRecieved);
        AbilitiesManager.GetAssetData(targetId).abilityManifest = JsonConvert.DeserializeObject<Dictionary<int, string>>(jsonManifest);               
    }

    public void SendManifest() {

        string abilityManifestPath = Path.Combine(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP, "AbilityManifest.json");

        if(File.Exists(abilityManifestPath)) {
            expectedDataCount = 1;
            string fileContents = File.ReadAllText(abilityManifestPath);
            SetBytesToSend(File.ReadAllBytes(abilityManifestPath));
        }        
    }
}

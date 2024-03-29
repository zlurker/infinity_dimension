﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System;

public class ImageDependenciesTransfer : NetworkMessageEncoder {

    Dictionary<int, string> currPath;
    public int expectedFiles;
    public int sentFiles;

    public void ResetTransfer() {
        currPath = new Dictionary<int, string>();
        expectedFiles = 0;
        sentFiles = 0;
    }

    public override void MessageRecievedCallback() {

        if(!currPath.ContainsKey(targetId))
            currPath.Add(targetId, "");

        if(currPath[targetId] == "") {
            currPath[targetId] = Encoding.Default.GetString(bytesRecieved);
            //Debug.Log("Path Size: " + bytesRecieved.Length);
        } else {
            Texture2D generatedTex = new Texture2D(1, 1);
            generatedTex.LoadImage(bytesRecieved);
            Sprite sprInst = Sprite.Create(generatedTex, new Rect(0, 0, generatedTex.width, generatedTex.height), new Vector2(0.5f, 0.5f));

            //Debug.LogFormat("Target: {0}. Curr Path: {1}.", targetId, currPath);
            //Debug.LogFormat("Client: ID{0}. Sprite Size: {1}", ClientProgram.clientId, bytesRecieved.Length);
            AbilitiesManager.GetAssetData(targetId).assetData.Add(currPath[targetId], sprInst);

            if(targetId == ClientProgram.clientId)
                sentFiles++;

            currPath[targetId] = "";
        }
    }

    public void SendArtAssets() {
        HashSet<string> assetPaths = new HashSet<string>();

        DirectoryBytesData dirData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(3);

        for (int i=0; i < dirData.filesData.Length; i++) {
            string jsonFile = Encoding.Default.GetString(dirData.filesData[i][0]);
            string[] imagePaths = JsonConvert.DeserializeObject<string[]>(jsonFile);

            for(int j = 0; j < imagePaths.Length; j++)
                if(!assetPaths.Contains(imagePaths[j]))
                    assetPaths.Add(imagePaths[j]);
        }

        string folderPath = FileSaver.PathGenerator(new string[] { LoadedData.gameDataPath, "UsrCreatedArt" });

        expectedFiles = assetPaths.Count;

        foreach(string path in assetPaths) {
            byte[] image = File.ReadAllBytes(Path.Combine(folderPath, path));
            SetBytesToSend(Encoding.Default.GetBytes(path));
            SetBytesToSend(image);
        }
    }
}

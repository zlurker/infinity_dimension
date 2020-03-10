using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System;

public class ImageDependenciesTransfer : NetworkMessageEncoder {

    string currPath = "";

    public override void MessageRecievedCallback() {
        if(currPath == "")
            currPath = Encoding.Default.GetString(bytesRecieved);
        else {
            Texture2D generatedTex = new Texture2D(1, 1);
            generatedTex.LoadImage(bytesRecieved);
            Sprite sprInst = Sprite.Create(generatedTex, new Rect(0, 0, generatedTex.width, generatedTex.height), new Vector2(0.5f, 0.5f));

            AbilitiesManager.GetAssetData(targetId).assetData.Add(currPath, sprInst);
            currPath = "";
        }
    }

    public void SendArtAssets() {
        HashSet<string> assetPaths = new HashSet<string>();
        byte[][][] cData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].ReturnAllMainFiles(new int[] { 7 });

        Debug.Log(cData.Length);

        for(int i = 0; i < cData.Length; i++) {
            string jsonFile = Encoding.Default.GetString(cData[i][0]);
            Debug.Log("json file: " + jsonFile);
            string[] imagePaths = JsonConvert.DeserializeObject<string[]>(jsonFile);

            for(int j = 0; j < imagePaths.Length; j++)
                if(!assetPaths.Contains(imagePaths[j]))
                    assetPaths.Add(imagePaths[j]);
        }

        string folderPath = FileSaver.PathGenerator(new string[] { Application.dataPath, "UsrCreatedArt" });

        //SetBytesToSend(BitConverter.GetBytes(assetPaths.Count));

        foreach(string path in assetPaths) {
            byte[] image = File.ReadAllBytes(Path.Combine(folderPath, path));
            SetBytesToSend(Encoding.Default.GetBytes(path));
            SetBytesToSend(image);
        }
    }
}

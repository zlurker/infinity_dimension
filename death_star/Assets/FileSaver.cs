using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSaver : MonoBehaviour
{

    public static void SaveFile(string value)
    {
        string path;

        path = PathGenerator(Application.dataPath,new string[] { "DataFiles" });

        path = Path.Combine(path,"TestFile");
        path += ".json";

        if (!File.Exists(path))
            File.Create(path).Dispose();

        File.WriteAllText(path, value);
    }

    public static string PathGenerator(string[] pathParam) {
        string path = "";

        for(int i = 0; i < pathParam.Length; i++) {
            path = Path.Combine(path, pathParam[i]);

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        return path;
    }

    public static string PathGenerator(string startPath,string[] pathParam) {
        string path = startPath;

        for(int i = 0; i < pathParam.Length; i++) {
            path = Path.Combine(path, pathParam[i]);

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        return path;
    }
}

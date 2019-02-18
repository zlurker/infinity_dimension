using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSaver : MonoBehaviour
{

    public static void SaveFile(string[] pathParam, string value)
    {
        string path = Application.dataPath;

        Debug.Log(pathParam.Length);
        for (int i = 0; i < pathParam.Length - 1; i++)
        {
            path = Path.Combine(path, pathParam[i]);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);           
        }

        path = Path.Combine(path,pathParam[pathParam.Length-1]);
        path += ".json";

        if (!File.Exists(path))
            File.Create(path).Dispose();

        File.WriteAllText(path, value);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSaveTemplate<T> : FileSaveTemplate {
    public Action<string, T> s;

    public FileSaveTemplate(string catergory, string[] filePath, Action<string, T> save) { //filePath followed by DataPath
        c = catergory;
        s = save;

        fP = FileSaver.PathGenerator(Application.dataPath, filePath);
    }
}

public class FileSaveTemplate {
    public string c;
    public string fP;

    public void GenericSaveTrigger<T>(string[] addtionalPath, T data) {
        string generatedPath = FileSaver.PathGenerator(fP,addtionalPath);

        (this as FileSaveTemplate<T>).s(generatedPath, data);
    }
}


public class FileSaver {
    public static FileSaveTemplate[] sFT = new FileSaveTemplate[] {
        new FileSaveTemplate<string>("Datafile", new string[]{ "Datafiles","Experimental" },(fP, t)=>{
        fP += ".json";

        if (!File.Exists(fP))
            File.Create(fP).Dispose();

        File.WriteAllText(fP, t);
        })
    };

    public static string PathGenerator(string[] pathParam) {
        string path = "";

        for(int i = 0; i < pathParam.Length; i++) {
            path = Path.Combine(path, pathParam[i]);

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        return path;
    }

    public static string PathGenerator(string startPath, string[] pathParam) {
        string path = startPath;

        for(int i = 0; i < pathParam.Length; i++) {
            path = Path.Combine(path, pathParam[i]);

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        return path;
    }
}

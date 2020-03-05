using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSaveTemplate<T> : FileSaveTemplate {
    public Action<string, T> s;

    public FileSaveTemplate(string[] filePath, string[] extension, string[] directories, Action<string, T> save) { //filePath followed by DataPath
        s = save;
        ext = extension;
        dir = directories;

        fP = FileSaver.PathGenerator(Application.dataPath, filePath);
    }
}

public class FileSaveTemplate {
    public string fP;
    public string[] ext;
    public string[] dir;

    public void GenericSaveTrigger<T>(string[] addtionalPath, int file, T data) {
        string generatedPath = FileSaver.PathGenerator(fP, addtionalPath);

        (this as FileSaveTemplate<T>).s(Path.Combine(generatedPath, ext[file]), data);
    }

    public string GenericLoadTrigger(string[] addtionalPath, int file) {
        string generatedPath = FileSaver.PathGenerator(fP, addtionalPath);
        generatedPath = Path.Combine(generatedPath, ext[file]);
        string text = "";

        using(StreamReader reader = new StreamReader(generatedPath)) {
            text = reader.ReadToEnd();
            reader.Close();
        }

        return text;
    }

    public string[] GenericLoadAll(int file) {
        DirectoryInfo[] currDirs = new DirectoryInfo(fP).GetDirectories();
        string[] files = new string[currDirs.Length];

        for(int i = 0; i < currDirs.Length; i++)
            files[i] = GenericLoadTrigger(new string[] { currDirs[i].Name }, file);

        return files;
    }

    public byte[][][] LoadAllDir(int d) {
        DirectoryInfo[] currDirs = new DirectoryInfo(fP).GetDirectories();
        byte[][][] files = new byte[currDirs.Length][][];

        for(int i = 0; i < currDirs.Length; i++) {
            DirectoryInfo currDir = new DirectoryInfo(Path.Combine(currDirs[i].FullName,dir[d]));
            FileInfo[] currDirFiles = currDir.GetFiles();
            files[i] = new byte[currDirFiles.Length][];

            for (int j=0; j < currDirFiles.Length; j++) 
                files[i][j] = File.ReadAllBytes(currDirFiles[j].FullName);            
        }

        return files;
    }

    public string[][] GetDirectoryNames(int d) {
        DirectoryInfo[] currDirs = new DirectoryInfo(fP).GetDirectories();
        string[][] fileNames = new string[currDirs.Length][];

        for(int i = 0; i < currDirs.Length; i++) {
            DirectoryInfo currDir = new DirectoryInfo(Path.Combine(currDirs[i].FullName, dir[d]));
            FileInfo[] currDirFiles = currDir.GetFiles();
            fileNames[i] = new string[currDirFiles.Length];

            for(int j = 0; j < currDirFiles.Length; j++)
                fileNames[i][j] = currDirFiles[j].Name;
        }

        return fileNames;
    }

    public void GenerateNewSubDirectory(string[] addtionalPath) {
        string path = FileSaver.PathGenerator(fP, addtionalPath);

        for(int i = 0; i < ext.Length; i++)
            File.Create(Path.Combine(path, ext[i])).Dispose();

        for(int i = 0; i < dir.Length; i++)
            Directory.CreateDirectory(Path.Combine(path, dir[i]));
    }
}

public struct FileSaverTypes {
    public const int PLAYER_GENERATED_DATA = 0;
}

public class FileSaver {

    //public static Dictionary<string, FileSaveTemplate> sFT;

    public static FileSaveTemplate[] sFT = new FileSaveTemplate[] {
         new FileSaveTemplate<string>(
             new string[]{ "Datafiles" },
             new string[]{"Ability.json","Info.json","WindowLocation.json","AbilityLauncher.json","NodeBranchData.json","SpecialisedNodeData.json","VariableBlockData.json"},
             new string[]{"ImageAssets"},
             (fP, t)=>{

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

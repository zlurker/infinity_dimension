using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

public struct DirectoryBytesData {
    public string[] dirName;
    public byte[][][] filesData;

    public DirectoryBytesData(string[] dName, byte[][][] bytes) {
        dirName = dName;
        filesData = bytes;
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

    public Dictionary<string,byte[][]> ReturnAllMainFiles(int[] selectedFiles) {
        Dictionary<string, byte[][]> dirData = new Dictionary<string, byte[][]>();
        //List<byte[][]> compiledList = new List<byte[][]>();
        //List<string> dirNames = new List<string>();
        var currDirs = new DirectoryInfo(fP).EnumerateDirectories();

        foreach(DirectoryInfo info in currDirs) {
            List<byte[]> compiledList = new List<byte[]>();
            //compiledList.Add(new byte[selectedFiles.Length][]);
            //dirNames.Add(info.Name);

            for (int i=0; i < selectedFiles.Length; i++) {
                string combinedPath = Path.Combine(info.FullName, ext[selectedFiles[i]]);
                compiledList.Add(File.ReadAllBytes(combinedPath));
            }

            dirData.Add(info.Name, compiledList.ToArray());
        }



        return dirData;
    }

    public string[] LoadAllDir(int d) {
        List<string> imagePaths = new List<string>();

        var currDirs = new DirectoryInfo(fP).EnumerateDirectories();

        foreach(DirectoryInfo info in currDirs) {
            DirectoryInfo currDir = new DirectoryInfo(Path.Combine(info.FullName, dir[d]));
            var currFiles = currDir.EnumerateFiles("*.PNG");

            foreach(FileInfo fInfo in currFiles)
                imagePaths.Add(fInfo.FullName);
        }

        return imagePaths.ToArray();
    }

    public string[][] GetDirectoryNames(int d) {
        DirectoryInfo[] currDirs = new DirectoryInfo(fP).GetDirectories();
        string[][] fileNames = new string[currDirs.Length][];

        for(int i = 0; i < currDirs.Length; i++) {
            DirectoryInfo currDir = new DirectoryInfo(Path.Combine(currDirs[i].FullName, dir[d]));
            FileInfo[] currDirFiles = currDir.GetFiles("*.PNG");
            fileNames[i] = new string[currDirFiles.Length];

            for(int j = 0; j < currDirFiles.Length; j++) {
                fileNames[i][j] = currDirFiles[j].Name;
                Debug.Log(fileNames[i][j]);
            }
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
             new string[]{"Ability.json","Info.json","WindowLocation.json","AbilityLauncher.json","NodeBranchData.json","SpecialisedNodeData.json","VariableBlockData.json","ImageDependencies.json"},
             new string[0],
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

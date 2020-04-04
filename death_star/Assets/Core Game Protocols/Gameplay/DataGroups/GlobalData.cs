using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;

public interface ISingleton {
    void RunOnStart();
    void RunOnCreated();
}

public interface IRPGeneric {
    void RunAccordingToGeneric<T,P>(P arg);
}

public enum VariableTypes {

    // Used by network
    CLIENT_ACTIVATED, HOST_ACTIVATED,

    // Used by variable connections
    PERMENANT_TYPE, SIGNAL_ONLY,

    IMAGE_DEPENDENCY, AUTO_MANAGED   
}

public class RuntimeParameters<T> : RuntimeParameters {
    public T v;

    public RuntimeParameters() {
    }

    public RuntimeParameters(string name, T value) {
        n = name;
        v = value;
        t = typeof(T);
        vI = VariableTypeIndex.ReturnVariableIndex(t);
    }

    public override string GetSerializedObject() {
        return JsonConvert.SerializeObject(this);
    }

    public override RuntimeParameters ReturnNewRuntimeParamCopy() {
        return new RuntimeParameters<T>(n, v);
    }

    public override void RunGenericBasedOnRP<P>(IRPGeneric inst, P arg) {
        inst.RunAccordingToGeneric<T,P>(arg);
    }
}

public class LoadedRuntimeParameters {
    public RuntimeParameters rP;
    public HashSet<VariableTypes> vT;

    public LoadedRuntimeParameters(RuntimeParameters runtimeParams) {
        rP = runtimeParams;
    }

    public LoadedRuntimeParameters(RuntimeParameters runtimeParams, params VariableTypes[] variableTypes) {
        rP = runtimeParams;
        vT = new HashSet<VariableTypes>(variableTypes);
    }
}

public class RuntimeParameters {

    public string n;
    public Type t;
    public int vI;

    public virtual string GetSerializedObject() {
        return "";
    }

    public virtual RuntimeParameters ReturnNewRuntimeParamCopy() {
        return null;
    }

    public virtual void RunGenericBasedOnRP<P>(IRPGeneric inst, P arg){
    }
}

public static class LoadedData {

    public static double connectionTimeOffset;
    public static double startTimeSinceConnection;

    public static Camera currSceneCamera;

    public static Dictionary<Type, ISingleton> singletonList;
    public static Dictionary<Type, AbilityTreeNode> loadedNodeInstance;
    public static Dictionary<Type, LoadedRuntimeParameters[]> loadedParamInstances;

    public static T GetSingleton<T>() {
        return (T)singletonList[typeof(T)];
    }

    public static double GetCurrentTimestamp() {
        return Time.realtimeSinceStartup - (connectionTimeOffset + startTimeSinceConnection);
    }

    public static RuntimeParameters[] ReturnNodeVariables(Type nodeT) {
        if(loadedParamInstances.ContainsKey(nodeT)) {

            RuntimeParameters[] rP = new RuntimeParameters[loadedParamInstances[nodeT].Length];

            for(int i = 0; i < rP.Length; i++)
                rP[i] = loadedParamInstances[nodeT][i].rP;

            return rP;
        }

        return null;
    }

    public static bool GetVariableType(Type t, int var, VariableTypes vTypes) {
        LoadedRuntimeParameters lRP = loadedParamInstances[t][var];

        if(lRP.vT != null)
            return lRP.vT.Contains(vTypes);

        return false;
    }
}

public static class SceneTransitionData {
    public static int sO; //sceneOffset

    public static void Initialise() {
        sO = 2;
        SceneManager.sceneLoaded += OnSceneLoad;
        LoadScene(new object[] { 0 });
    }

    public static void LoadScene(object[] p) {
        int sI = (int)p[0] + sO;
        SceneManager.LoadScene(sI);
    }

    public static void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public static void OnSceneLoad(Scene arg0, LoadSceneMode arg1) {
        LoadedData.currSceneCamera = Camera.main;

        foreach(KeyValuePair<Type, ISingleton> singletons in LoadedData.singletonList)
            singletons.Value.RunOnStart();
    }
}
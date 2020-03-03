﻿using System.Collections;
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

public enum VariableTypes {
    DEFAULT, LINKS_NOT_CALCULATED
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
}

public class RuntimeParameters{

    public string n;
    public Type t;

    public virtual string GetSerializedObject() {
        return "";
    }
    public int vI;
}

public static class LoadedData {

    //public static IPlayerEditable[] uL; //uiLoaders
    //public static Singleton[] sL; //singletonList
    public static Dictionary<Type, ISingleton> singletonList;
    public static Dictionary<Type, AbilityTreeNode> loadedNodeInstance;

    public static T GetSingleton<T>() {
        return (T)singletonList[typeof(T)];
    }
    //public static InterfaceLoader[] lI; //loadedInterfaces
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
        foreach (KeyValuePair<Type, ISingleton> singletons in LoadedData.singletonList) 
            singletons.Value.RunOnStart();       
    }
}
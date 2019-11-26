using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;

#region Gameplay Data Structures

/*public interface IPlayerEditable
{
    RuntimeParameters[] GetRuntimeParameters();
    void SetValues(RuntimeParameters[] runtimeParameters);
}*/

public interface ISingleton
{
    void RunOnStart();
    void RunOnCreated();
}

public interface ISpawnable
{
}


public class RuntimeParameters<T>:RuntimeParameters {
    public T v;

    public RuntimeParameters() {
    }

    public RuntimeParameters(string name,T value) {       
        n = name;
        v = value;
        t = typeof(T);
        vI = VariableTypeIndex.ReturnVariableIndex(t);
    }

    public override string GetSerializedObject() {
        return JsonConvert.SerializeObject(this);
    }
}

public class RuntimeParameters : Iterator {

    public virtual string GetSerializedObject() {
        return "";
    }
    public int vI;
}

public struct EffectTemplate
{
    public string statAffected;
    public float duration;
    public float tickCount;
    public string operation;
    public float value;
    public bool permanent;

    public EffectTemplate(string sA, float d, float tC, string o, float v, bool p)
    {
        statAffected = sA;
        duration = d;
        tickCount = tC;
        operation = o;
        value = v;
        permanent = p;
    }
}

public class Stat : Iterator
{
    public float v;
    public float pTC;

    public Stat(string name, float val)
    {
        n = name;
        v = val;
    }
}

public struct PointData
{
    public float aC; //angleChanges
    public float u; //unit

    public PointData(float angleChanges, float unit)
    {
        aC = angleChanges;
        u = unit;
    }
}
#endregion

#region General Data Structures
public delegate void r(object[] p);

[System.Serializable]
public class Iterator
{
    public string n;
    public Type t;

    public Iterator()
    {
        if (string.IsNullOrEmpty(n))
            if (t != null)
                n = t.Name;
    }


    public static int ReturnKey(Iterator[] tA, string k)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(tA[i].n, k))
                return i;

        return -1;
    }

    public static int ReturnKey<T>(T[] tA, string k,Func<T,string> iI)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(iI(tA[i]), k))
                return i;

        return -1;
    }

    public static int ReturnKey<T1,T2>(T1[] tA, T2 k, Func<T1, T2> iI) {
        for(int i = 0; i < tA.Length; i++)
            if(object.Equals(iI(tA[i]),k))
                return i;

        return -1;
    }

    public static T ReturnObject<T>(T[] tA, Type k, Func<T, Type> iI) {
        
        for(int i = 0; i < tA.Length; i++)
            if(k == iI(tA[i]))
                return tA[i];

        return (T) (null as object);
    }

    public static T ReturnObject<T>(T[] tA, string k, Func<T, string> iI)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(iI(tA[i]), k))
                return (T)(tA[i] as object);

        return (T)(null as object);
    }

    public static T ReturnObject<T>(T[] tA, Func<T,bool> iI)
    {
        for (int i = 0; i < tA.Length; i++)
            if (iI(tA[i]))
                return (T)(tA[i] as object);

        return (T)(null as object);
    }

    public static T ReturnObject<T>(Iterator[] tA, string k)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(tA[i].n, k))
                return (T)(tA[i] as object);

        return (T)(null as object);
    }

    public static Iterator ReturnObject<T>(Iterator[] tA)
    {
        for (int i = 0; i < tA.Length; i++)
            if (tA[i].t == typeof(T))
                return (Iterator)(tA[i] as object);
        return (Iterator)(null as object);
    }

    public static Iterator ReturnObject(Iterator[] tA, Type t)
    {
        for (int i = 0; i < tA.Length; i++)
            if (tA[i].t == t)
                return (Iterator)(tA[i] as object);
        return (Iterator)(null as object);
    }
}

public class DH
{ //delegateHelper
    public r d; //delegate
    public object[] p; //parameters

    public DH(r del)
    {
        d = del;
        p = new object[0];
    }

    public DH(r del, object[] parameters)
    {
        d = del;
        p = parameters;
    }

    public void Invoke()
    { //Fixed parameters
        d(p);
    }

    public void Invoke(object[] parameters)
    { //For custom parameters
        d(parameters);
    }
}

#endregion

public static class GlobalData
{
    #region Gameplay Global Data

    #endregion

    //Loads a new level and refreshes data structures if needed.
    /*    public static void LoadNewLevel(int level)
        {
            DelegatePools.ClearDelegatePools();
            SceneManager.LoadScene(level);
        }*/
}

public class Singleton : Iterator
{
    public ISingleton linkedInstance;

    public Singleton(ISingleton singleton)
    {
        linkedInstance = singleton;
        t = linkedInstance.GetType();
    }

    public static T GetSingleton<T>()
    {
        Singleton instance = ReturnObject(LoadedData.sL, typeof(T)) as Singleton;
        return (T)instance.linkedInstance;
    }

    public static ISingleton GetSingleton(Type t)
    {
        Singleton instance = ReturnObject(LoadedData.sL, t) as Singleton;
        return instance.linkedInstance;
    }
}

public static class LoadedData
{
    //public static IPlayerEditable[] gIPEI; //globalIPlayerEditableInstances
    //public static IPlayerEditable[] uL; //uiLoaders
    public static Singleton[] sL; //singletonList
    public static InterfaceLoader[] lI; //loadedInterfaces
}

public static class SceneTransitionData
{
    public static int sO; //sceneOffset

    public static void Initialise()
    {
        sO = 2;
        SceneManager.sceneLoaded += OnSceneLoad;
        LoadScene(new object[] { 0 });
    }

    public static void LoadScene(object[] p)
    {
        int sI = (int)p[0] + sO;
        SceneManager.LoadScene(sI);
    }

    public static void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public static void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        ISingleton[] interfaces = (Iterator.ReturnObject<ISingleton>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as ISingleton[];

        for (int i = 0; i < interfaces.Length; i++)
            interfaces[i].RunOnStart(); //Runs all the singleton start  
    }
}

public static class DelegatePools
{
    public static List<DH> jD; //judgementDelegate

    public static void ClearDelegatePools()
    {
        jD = new List<DH>();
    }
}

public static class PresetGameplayData
{
    public static Stat[] sT = new Stat[] {
        new Stat("Current Health", 50),
        new Stat("Max Health", 50),
        new Stat("Movespeed", 1),
        new Stat("Health Regeneration", 1)  };

    /*public static ClassFilter[] standaloneClass = new ClassFilter[] {
        new ClassFilter<Type>(typeof(ISpawnable), (t)=>{
            ScriptableObject scriptableObject = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[]{ t});
            return Spawner.GetCType(scriptableObject,t) as IPlayerEditable;
        }),
        new ClassFilter<Type>(typeof(ISingleton), (t) => {
            return Singleton.GetSingleton(t) as IPlayerEditable;
        })
    };*/


}

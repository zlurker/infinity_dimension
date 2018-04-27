using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

#region Gameplay Data Structures
public enum RhythmAnalyseState
{
    Manual, Auto
}

public interface IPlayerEditable
{
    MethodInfo GetMainMethod();
    void Invoke(object[] p);
}

public interface ISingleton
{
    void RunOnStart();
    void RunOnCreated();
    object ReturnInstance();
}

public interface IStarters
{

}

public interface IComponent
{

}

public class CustomClassFirer : Iterator
{
    public List<object[]> p; //Allows mutiple methods to be called. Every one object[] is one method. 

    public CustomClassFirer(object[] parameters, string name)
    {
        p = new List<object[]>();
        p.Add(parameters);
        n = name; //In this case, it is the name of the class.
    }
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

public struct JudgementRange
{
    public string name;
    public float maxWindow;
    public int counter;

    public JudgementRange(string n, float mW, int c)
    {
        name = n;
        maxWindow = mW;
        counter = c;
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

/*[System.Serializable]
public class UIElement : BaseIterator {
    public MaskableGraphic u;
    public float dAT;
    

    public UIElement(string name, MaskableGraphic ui, float deActivateTiming) {
        n = name;
        u = ui;
    }
}*/

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

    public static int ReturnKey(Iterator[] tA, string k)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(tA[i].n, k))
                return i;

        //Debug.LogErrorFormat("The key: {0} does not exist.", k);
        return -1;
    }

    public static T ReturnObject<T>(Iterator[] tA, string k)
    {
        for (int i = 0; i < tA.Length; i++)
            if (string.Equals(tA[i].n, k))
                return (T)(tA[i] as object);

        //Debug.LogErrorFormat("The key: {0} does not exist.", k);
        return (T)(null as object);
    }

    public static Iterator ReturnObject<T>(Iterator[] tA)
    {
        for (int i = 0; i < tA.Length; i++)
            if (tA[i].t == typeof(T))
                return (Iterator)(tA[i] as object);
        return (Iterator)(null as object);
    }

    public static Iterator ReturnObject(Iterator[] tA,Type t)
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

public class DelegateIterator : Iterator
{
    public DH d;

    public DelegateIterator(string name, DH deleg)
    {
        n = name;
        d = deleg;
    }
}


/*public class MethodRunner<T> : MethodRunner
{
    public Action<T, object[]> d; //delegate

    public MethodRunner(Action<T, object[]> del)
    {
        d = del;
    }

    public override void Run(object[] args)
    {
        d(args);
    }
}*/

/*public class MethodRunner
{
    public virtual void Run(object[] args)
    {

    }
}*/

/*public class PointerGroup : Iterator
{
    public PointerHolder[] cP;//classPointers;

    public PointerGroup(string scriptName, PointerHolder[] classPointers)
    {
        n = scriptName;
        cP = classPointers;
    }
}*/



/*public class PointerHolderCreatorBase
{
    public PointerHolder[] p;//pointers
}*/
#endregion


public static class GlobalData
{
    #region Gameplay Global Data
    public static float bpm;
    public static float offset;
    public static bool followsBeat = false;
    public static RhythmAnalyseState analyseAlgorithm;

    public static AudioClip song;

    #endregion

    //Loads a new level and refreshes data structures if needed.
    /*    public static void LoadNewLevel(int level)
        {
            DelegatePools.ClearDelegatePools();
            SceneManager.LoadScene(level);
        }*/
}

public static class LoadedData
{
    public static IPlayerEditable[] gIPEI; //globalIPlayerEditableInstances
    public static IPlayerEditable[] uL; //uiLoaders
    public static ISingleton[] sL; //singletonList
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

    public static void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < LoadedData.sL.Length; i++)
            LoadedData.sL[i].RunOnStart(); //Runs all the singleton start  
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

public static class BaseIteratorFunctions
{ //A list of functions that complements the BaseIterator class

    //Iterates though the Array and returns the first item with the string k

}

public static class PresetGameplayData
{
    public static Stat[] sT = new Stat[]
  { new Stat("Current Health", 50), new Stat("Max Health", 50), new Stat("Movespeed", 1), new Stat("Health Regeneration", 1)  };

    public static JudgementRange[] jRT = new JudgementRange[]
    { new JudgementRange("YEAH!", 0.05f, 0), new JudgementRange("SUPER", 0.1f, 0), new JudgementRange("GOOD", 0.2f, 0), new JudgementRange("OK", 0.3f, 0) };
}

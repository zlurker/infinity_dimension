using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class UIDrawer : Spawner, ISingleton {
    public static Canvas t; //target
    public static Dictionary<Type, int> butInpIds;
    public static Dictionary<Type, Dictionary<string, int>> uiWrapperDir;

    public override SpawnerOutput CreateScriptedObject(Type type) {
        SpawnerOutput inst = base.CreateScriptedObject(type);
        inst.script.transform.SetParent(t.transform);
        return inst;
    }

    public static Vector3 UINormalisedPosition(Vector3 c) {//coordinates: Returns back position to the decimal of 1.
        return UINormalisedPosition(t.transform as RectTransform, c);
    }

    public static Vector3 UINormalisedPosition(RectTransform target, Vector2 c) {//coordinates: Returns back position to the decimal of 1.
        c -= target.pivot;
        for(int i = 0; i < 2; i++) {
            c[i] = (c[i] / 1) * target.sizeDelta[i];
            c[i] += target.position[i];
        }
        return c;
    }

    /*public static T GetTypeInElement<T>(SpawnerOutput t) {

        if(t.script is T)
            return (T)(object)t.script;

        UIWrapperBase target = t.script as UIWrapperBase;

        // Deals with UIWrapperBase objects.
        if(target != null) {
            if(target.mainScript is T)
                return (T)(object)target.mainScript;

            if(target.mainScript is Button || target.mainScript is InputField) {
                MonoBehaviour inst = target.additionalScripts[butInpIds[typeof(T)]].script;

                if(inst is UIWrapperBase)
                    return (T)(object)(inst as UIWrapperBase).mainScript;

                return (T)(object)target.additionalScripts[butInpIds[typeof(T)]].script;
            }
        }

        return (T)(object)null;
    }

    public static T GetTypeInWrapper<T>(SpawnerOutput t, string cN) {

        UIWrapperBase target = t.script as UIWrapperBase;

        if(target != null) {
            if(cN == "")
                return (T)(object)target.mainScript;

            if(!uiWrapperDir.ContainsKey(target.GetType())) {
                Dictionary<string, int> uiDir = new Dictionary<string, int>();

                target.PopulateScriptDirectory(uiDir);
                uiWrapperDir.Add(target.GetType(), uiDir);
            }

            return (T)(object)target.additionalScripts[uiWrapperDir[target.GetType()][cN]];
        }

        /*if(target != null) {

            if(target.mainScript is Button || target.mainScript is InputField) {
                MonoBehaviour inst = target.additionalScripts[butInpIds[typeof(T)]].script;

                if(inst is UIWrapperBase)
                    return (T)(object)(inst as UIWrapperBase).mainScript;

                return (T)(object)target.additionalScripts[butInpIds[typeof(T)]].script;
            }
        }

        return (T)(object)null;
    }*/

    public static void ChangeUISize(SpawnerOutput t, Vector2 size) {

        (t.script.transform as RectTransform).sizeDelta = size;

        UIWrapperBase target = t.script as UIWrapperBase;

        if(target != null)
            for(int i = 0; i < target.additionalScripts.Length; i++)
                (target.additionalScripts[i].script.transform as RectTransform).sizeDelta = size;
    }

    public new void RunOnStart() {
        t = FindObjectOfType<Canvas>();
        uiWrapperDir = new Dictionary<Type, Dictionary<string, int>>();
    }

    public new void RunOnCreated() {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

        // Runs data population for UIWrappers.
        butInpIds = new Dictionary<Type, int>();
        butInpIds.Add(typeof(Image), 0);
        butInpIds.Add(typeof(Text), 1);
    }

    /*public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","What the fuck nigga")
        };
    }*/
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class UIDrawer : Spawner, ISingleton {
    public static Canvas t; //target
    public static Dictionary<Type, int> butInpIds;


    public override SpawnerOutput CreateScriptedObject(Type type) {
        SpawnerOutput inst = base.CreateScriptedObject(type);
        inst.script.transform.SetParent(t.transform);
        return inst;
    }

    // A wrapped to assist us in getting elements nested within.


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

    public static T GetTypeInElement<T>(SpawnerOutput t) {

        if(t.script is T)
            return (T)(object)t.script;

        UIWrapperBase target = t.script as UIWrapperBase;

        if(target != null) 
            if(target.mainScript is Button || target.mainScript is InputField)
                return (T)(object)target.additionalScripts[butInpIds[typeof(T)]].script;
        

        return (T)(object)null;
    }

    public static void ChangeUISize(SpawnerOutput t, Vector2 size) {

        (t.script.transform as RectTransform).sizeDelta = size;

        UIWrapperBase target = t.script as UIWrapperBase;

        if(target != null)
            for(int i = 0; i < target.additionalScripts.Length; i++)
                (target.additionalScripts[i].script.transform as RectTransform).sizeDelta = size;
    }

    public new void RunOnStart() {
        t = FindObjectOfType<Canvas>();
    }

    public new void RunOnCreated() {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

        // Runs data population for UIWrappers.
        butInpIds = new Dictionary<Type, int>();
        butInpIds.Add(typeof(Image), 0);
        butInpIds.Add(typeof(Text), 1);
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","What the fuck nigga")
        };
    }
}
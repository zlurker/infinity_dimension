﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIDrawer : Spawner, ISingleton, IPlayerEditable {

    public static UIDrawer i;
    public static Canvas t; //target

    public override void CreateTypePool() {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

        tP = new TypePool[] {
            new TypePool(new Type[] { typeof(Image) }, "Image"),
            new TypePool(new Type[] { typeof(Image), typeof(Button)},"Button"),
            new TypePool(new Type[] { typeof(Text)}, "Text")
        };
    }

    //Button test;

    void Start() {
        //Spawn("Button");
        //Debug.DrawLine(new Vector3(), ReturnPosition(new Vector2(0.7f, 0.25f)), Color.red, 5f);
    }

    //public void SpawnWithPointData(string p, string pDN) { //pool, pointDataName
    //Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
    // }

    //public void SpawnWithPointData(string p, string pDN, float d) { //pool, pointDataName, duration
    //  PoolElement i = Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
    //TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { i })));
    //}

    public override PoolElement Spawn(string p) { //pool
        return Spawn(p, new Vector3(), t.transform);
    }

    public override PoolElement Spawn(string p, Vector3 l) { //pool, location
        return Spawn(p, l, t.transform);
    }

    public override PoolElement Spawn(string p, Vector3 l, float d) { //pool, location, duration
        PoolElement iR = Spawn(p, l, t.transform);
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));

        return iR;
    }

    public Vector3 ReturnPosition(Vector3 c) {//coordinates: Returns back position to the decimal of 1.

        for (int i = 0; i < 2; i++) {
            c[i] = (c[i] / 1) * t.pixelRect.size[i];
            c[i] += t.pixelRect.min[i];
        }

        return c;
    }

    public void SetUIComponent(MonoBehaviour o, object p) {

        if (o is Text)
            (o as Text).text = p as string;

        if (o is Image)
            (o as Image).sprite = p as Sprite;

        //For button, parameter is a new DH.
        if (o is Button)
            (o as Button).onClick.AddListener((p as DH).Invoke);
    }

    public void SetUIComponent(MonoBehaviour[] o, Type t,object p) {
        for (int i = 0; i < o.Length; i++) 
            if (o[i].GetType() == t) 
                SetUIComponent(o[i], p);        
    }

    public object GetUIComponent(MonoBehaviour obj) {

        if (obj is Text)
            return (obj as Text).text;

        if (obj is Image)
            return (obj as Image).sprite;

        if (obj is InputField)
            return (obj as InputField).text;

        return null;
    }

    public void ReplaceUIGroup(string name, PoolElement[] replaceWith) {
        PatternControl.i.Pattern_Args(replaceWith,
            new object[][] {
                new object[] { "GROUPPATTERN", name, "REMOVE_ALL_CURRENT_OBJECTS,ADD_PARAMETER_OBJECTS"}
            }
            );
    }

    public void Fire(object[] parameters) {

    }

    public void LoadUI() {
        Debug.Log("TEST WORKS?");
    }

    public object ReturnInstance() {
        return i;
    }

    public void RunOnStart() {
        t = FindObjectOfType<Canvas>();
    }

    public void RunOnCreated() {
        i = this;
        DontDestroyOnLoad(gameObject);
    }
}
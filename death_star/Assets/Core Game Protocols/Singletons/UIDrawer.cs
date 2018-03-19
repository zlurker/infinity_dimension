using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetOnSpawnParameters {
    public Type t; //type
    public object v; //value

    public SetOnSpawnParameters(Type type, object value) {
        t = type;
        v = value;
    }
}

public class TextSettings : ObjectSettings {
    public override Modifier[] SetDefaultValues() {
        return new Modifier[] {
            new Modifier(0,"Default Text"),
            new Modifier(1,Resources.Load("jd-bold") as Font)
        };
    }

    public override void SetObjectValues(MonoBehaviour t, Modifier[] mods) {
        Text s = t as Text;
        for (int i = 0; i < mods.Length; i++)
            switch (mods[i].v) {
                case 0:
                    s.text = mods[i].vV as string;
                    break;
                case 1:
                    s.font = mods[i].vV as Font;
                    break;
            }
    }
}

public class UIDrawer : Spawner, ISingleton, IPlayerEditable {

    public static UIDrawer i; //instance
    public static Canvas t; //target

    public override void CreateTypePool() {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

        tP = new TypePool[] {
            new TypePool(new TypeIterator[] { new TypeIterator(typeof(Image)) }, "Image"),
            new TypePool(new TypeIterator[] { new TypeIterator(typeof(Image)), new TypeIterator(typeof(Button))},"Button"),
            new TypePool(new TypeIterator[] { new TypeIterator(typeof(Text),new TextSettings())}, "Text")
        };
    }

    public override PoolElement Spawn(string p) { //pool
        return Spawn(p, new Vector3(), t.transform);
    }

    public override PoolElement Spawn(string p, Vector3 l, float d) { //pool, location, duration
        PoolElement iR = Spawn(p, l, t.transform);
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));

        return iR;
    }

    public PoolElement Spawn(string p, bool uNP, Vector3 l) { //pool, location, useNormalisedPosition
        if (uNP)
            l = UINormalisedPosition(l);

        //return Spawn(p, l, t.transform);
        return Spawn(p, l, t.transform);
    }

    public PoolElement Spawn(string p, bool uNP, Vector3 l, SetOnSpawnParameters[] sP) { //pool, location, useNormalisedosition, spawnParameters
        if (uNP)
            l = UINormalisedPosition(l);

        PoolElement iR = Spawn(p, l, t.transform);

        for (int i = 0; i < sP.Length; i++)
            SetUIComponent(iR.o, sP[i].t, sP[i].v);

        return iR;
    }

    public Vector3 UINormalisedPosition(Vector3 c) {//coordinates: Returns back position to the decimal of 1.

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

    public void SetUIComponent(MonoBehaviour[] o, Type t, object p) {
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
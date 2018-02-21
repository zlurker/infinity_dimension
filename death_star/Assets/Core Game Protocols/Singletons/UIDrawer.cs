using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawer : Spawner, Singleton {

    public static UIDrawer i;
    public static Canvas t; //target

    void Start() {
        i = this;
        DontDestroyOnLoad(gameObject);
        RunOnStart();

        Debug.DrawLine(new Vector3(), ReturnPosition(new Vector2(0.7f, 0.25f)), Color.red, 5f);
    }

    //public void SpawnWithPointData(string p, string pDN) { //pool, pointDataName
    //Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
    // }

    //public void SpawnWithPointData(string p, string pDN, float d) { //pool, pointDataName, duration
    //  PoolElement i = Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
    //TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { i })));
    //}

    public override PoolElement Spawn(string p) { //pool, location
        return Spawn(p, new Vector3(), t.transform);
    }

    public override PoolElement Spawn(string p, Vector3 l) { //pool, location
        return Spawn(p, l, t.transform);
    }

    public override PoolElement Spawn(string p, Vector3 l, float d) { //pool, location
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

    public void ReplaceUIGroup(string name, PoolElement[] replaceWith) {
        PatternControl.i.Pattern_Args(replaceWith,
            new object[][] {
                new object[] { "GROUPPATTERN", name, "REMOVE_ALL_CURRENT_OBJECTS,ADD_PARAMETER_OBJECTS"}
            }
            );
    }

    public void RunOnStart() {
        t = FindObjectOfType<Canvas>();
    }
}
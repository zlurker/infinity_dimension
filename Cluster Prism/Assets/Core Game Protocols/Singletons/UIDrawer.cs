using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawer : Spawner {

    //public List<UIElement> uE;
    public static UIDrawer i;

    void Start() {
        i = this;
        DontDestroyOnLoad(gameObject);
        SpawnWithPointData("TEST", "TEST",5);
    }

    public void SpawnWithPointData(string p, string pDN) { //pool, pointDataName
        Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
    }

    public void SpawnWithPointData(string p, string pDN,float d) { //pool, pointDataName, duration
        MonoBehaviour inst = Spawn(p, UIData.i.ReturnPoint(pDN), UIData.i.t.transform);
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] {inst , p})));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandler : MonoBehaviour {

    public static TimeHandler i; //instance
    List<TimeData> tE; //timeEvents;

    void Start() {
        i = this;
        tE = new List<TimeData>();
        DontDestroyOnLoad(this);

        AddNewTimerEvent(new TimeData("Test", 5));
        AddNewDelegates("Test",RunNothing);
    }

    void RunNothing() {
        Debug.Log("Test");
    }

    void Update() {
        for (int i = 0; i < tE.Count; i++)
            if (tE[i].eT < Time.time) {
                for (int j = 0; j < tE.Count; j++)
                    tE[i].tD[j]();

                Debug.LogFormat("Function has been fired for {0} timer event. ", tE[i].n);
                tE.RemoveAt(i);
                Debug.LogFormat("Time events remaining: {0}", tE.Count);
            }
    }

    public void AddNewTimerEvent(TimeData d) {
        tE.Add(d);
    }

    public void SetEndTime(string n, float eT) {
        int i = BaseIteratorFunctions.IterateKey(tE.ToArray(), n);
        TimeData inst = tE[i];
        inst.eT = eT;

        tE[i] = inst;
    }

    public void AddNewDelegates(string n, r eD) {
        int i = BaseIteratorFunctions.IterateKey(tE.ToArray(), n);
        TimeData inst = tE[i];
        inst.tD.Add(eD);

        tE[i] = inst;
    }

    public float ReturnGameTimeUnit(float start, float interval) {
        return (Time.time - start) / interval;
    }

    public float ReturnRealTimeUnit(float start, float interval) {
        return (Time.realtimeSinceStartup - start) / interval;
    }
}

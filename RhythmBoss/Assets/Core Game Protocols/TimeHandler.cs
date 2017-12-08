using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandler : MonoBehaviour {

    public static TimeHandler i; //instance
    List<TimeData> tE; //timeEvents;

    void Start () {
        i = this;
        tE = new List<TimeData>();
        DontDestroyOnLoad(this);
	}
	
	void Update () {
	}


    public float ReturnGameTimeUnit(float start,float interval) {
        return (Time.time - start) / interval;
    }

    public float ReturnRealTimeUnit(float start, float interval) {
        return (Time.realtimeSinceStartup - start) / interval;
    }
}

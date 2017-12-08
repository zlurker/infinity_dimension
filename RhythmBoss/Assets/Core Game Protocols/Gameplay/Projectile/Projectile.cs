using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Projectile : MonoBehaviour {

    public float cA; //currAngle
    public PointData[] aC; //angleChanges
    public float tD; //totalDistance
    public float tT; //totalTime
    public int pF; //patternFrequency

    //float i; //interval
    int aCG; //actualChanges
    float sT; //startTime
    Vector3 sV; //startVector

    void Start() {
        cA = 0;
        aC = new PointData[] { new PointData(0, 0.2f), new PointData(50, 0.8f) };
        tD = 100;
        tT = 10;
        pF = 3;

        int tMD = aC.Length * pF;
        //i = tT / tMD;

        aCG = 0;
        //Debug.Log(i);
        sT = Time.time;
        sV = transform.position;
    }

    void Update() {
        float tI = tT * aC[aCG % aC.Length].u;
        float t = TimeHandler.i.ReturnGameTimeUnit(sT, tI);

        //Debug.Log(t);
        //int cC = Mathf.FloorToInt(Time.time / i);
        transform.position = sV + (Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u) * t);
        Debug.Log(cA);
        if (t > 1) {
            sV += Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u);
            //for (int j = aCG; j < cC; j++) {
            //Do stuff with angle data here.
            //Debug.LogFormat("Actual change is: {0}. Reading from array value: {1}", j, j % aC.Length);
            //}
            //(Time.time % i)/i
            sT += tI;
            aCG++;
            cA += aC[aCG % aC.Length].aC;
        }
    }
}

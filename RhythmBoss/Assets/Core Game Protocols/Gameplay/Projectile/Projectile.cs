using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float cA; //currAngle
    public float[] aC; //angleChanges
    public float tD; //totalDistance
    public float tT; //totalTime
    public int pF; //patternFrequency

    float i; //interval
    int aCG; //actualChanges
    Vector3 cV; //currVector

    void Start() {
        cA = 0;
        aC = new float[] { 20, -50, 70, 80, 100, -50 };
        tD = 100;
        tT = 10;
        pF = 3;

        int tMD = aC.Length * pF;
        i = tT / tMD;

        aCG = 0;
        cV = Math.VectorFromAngle(cA);
        Debug.Log(i);
    }

    void Update() {
        transform.position += cV;

        int cC = Mathf.FloorToInt(Time.time / i);
        
        if (cC > aCG)
            for (int j = aCG; j < cC; j++) {
                //Do stuff with angle data here.
                Debug.LogFormat("Actual change is: {0}. Reading from array value: {1}", j, j % aC.Length);
            }
        //(Time.time % i)/i
        aCG = cC;
    }
}

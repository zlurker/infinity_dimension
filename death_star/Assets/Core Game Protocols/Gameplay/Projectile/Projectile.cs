using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Projectile : MonoBehaviour,ISpawnable,IPlayerEditable
{

    public float cA; //currAngle
    public PointData[] aC; //angleChanges
    public float tD; //totalDistance
    public float tT; //totalTime
    public int pF; //patternFrequency

    //float i; //interval
    int aCG; //actualChanges
    float sT; //startTime
    Vector3 sV; //startVector

    void Start()
    {
        cA = 0;
        aC = new PointData[] { new PointData(0, 0.2f), new PointData(50, 0.8f) };
        tD = 100;
        tT = 10;
        pF = 3;

        aCG = 0;
        sT = Time.time;
        sV = transform.position;
    }

    void Update()
    {
        float tI = tT * aC[aCG % aC.Length].u;
        float t = Singleton.GetSingleton<TimeHandler>().ReturnGameTimeUnit(sT, tI);

        /*transform.position = sV + (Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u) * t);

        if (t > 1)
        {
            sV += Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u);
            sT += tI;
            aCG++;
            cA += aC[aCG % aC.Length].aC;
        }*/
    }

    public void OnSpawn()
    {
       
    }

    public RuntimeParameters[] GetRuntimeParameters() {

        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Name of Projectile","Marcus Warts"),
            new RuntimeParameters<float>("Projectile Speed", 5),
            new RuntimeParameters<float>("Projectile Damage", 20),
            new RuntimeParameters<int>("Multiplier", 15),
            new RuntimeParameters<EditableLinkInstance>("On Hit",new EditableLinkInstance(new SavedData[0]))
        };
    }

    public void SetValues(RuntimeParameters[] values) {
        Debug.Log("Projectile printing");
    }
}

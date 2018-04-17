﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Projectile : MonoBehaviour, IPlayerEditable
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
        float t = TimeHandler.i.ReturnGameTimeUnit(sT, tI);

        transform.position = sV + (Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u) * t);

        if (t > 1)
        {
            sV += Math.VectorFromAngle(cA) * (tD * aC[aCG % aC.Length].u);
            sT += tI;
            aCG++;
            cA += aC[aCG % aC.Length].aC;
        }
    }

    void DestroyProjectile()
    {
        Debug.Log("Destroying");
        //GameObjectSpawner.i.Remove(this, "Projectile");
       
    }

    public void SetProjectile(int test,int test2)
    {

    }

    public void Invoke(object[] test) //Need to just write down the method with all the paramters.
    {
        SetProjectile((int)test[0], (int)test[1]);
    }

    public MethodInfo GetMainMethod() 
    {
        return GetType().GetMethod("SetProjectile");
    }
}

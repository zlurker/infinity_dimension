﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPool : BaseIterator {
    public Pool<MonoBehaviour> sP;

    public SpawnerPool(string name, CI instanceCreator) {
        n = name;
        sP = new Pool<MonoBehaviour>(instanceCreator, name);
    }
}

[System.Serializable]
public class SpawnInstance : BaseIterator {
    public MonoBehaviour i;
}

public class Spawner : MonoBehaviour {
    
    public SpawnInstance[] iS; //instanceS

    SpawnerPool[] sP; //spawnPool;
    public OnSpawn test;
    int cK; //currentKey

    void Awake() {
        sP = new SpawnerPool[iS.Length];

        for (int i = 0; i < sP.Length; i++)
            sP[i] = new SpawnerPool(iS[i].n, CreateNewObject);

        //i = this;
        //Spawn("Projectile", new Vector3());
    }

    public MonoBehaviour Spawn(string p) { //pool
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        MonoBehaviour iR = sP[cOK].sP.Retrieve();
        iR.gameObject.SetActive(true);

        //(iR as OnSpawn).RunOnActive();
        return iR;
    }

    public MonoBehaviour Spawn(string p, Vector3 l) { //pool, location
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        MonoBehaviour iR = sP[cOK].sP.Retrieve();
        iR.gameObject.SetActive(true);
        iR.transform.position = l;

        //(iR as OnSpawn).RunOnActive();
        return iR;
    }

    public MonoBehaviour Spawn(string p, Vector3 l,Transform t) { //pool, location, target
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        MonoBehaviour iR = sP[cOK].sP.Retrieve();
        iR.gameObject.SetActive(true);

        iR.transform.parent = t;
        iR.transform.position = l;

        //(iR as OnSpawn).RunOnActive();
        return iR;
    }

    public void Remove(MonoBehaviour iR, string p) {
        iR.gameObject.SetActive(false);
        sP[BaseIteratorFunctions.IterateKey(sP, p)].sP.Store(iR);
    }

    public void Remove(object[] p) {
        Remove(p[0] as MonoBehaviour, p[1] as string);
        MonoBehaviour iR = p[0] as MonoBehaviour;
    }

    Object CreateNewObject() {
        return Instantiate(iS[cK].i);
    }
}

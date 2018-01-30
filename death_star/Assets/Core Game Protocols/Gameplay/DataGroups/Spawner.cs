using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPool : BaseIterator {
    public Pool<PoolElement> sP;

    public SpawnerPool(string name, CI instanceCreator) {
        n = name;
        sP = new Pool<PoolElement>(instanceCreator, name);
    }
}

public class PoolElement : BaseIterator {
    public MonoBehaviour o; //object

    public PoolElement(MonoBehaviour obj, string name) {
        o = obj;
        n = name;
    }
}

[System.Serializable]
public class SpawnInstance : BaseIterator {
    public MonoBehaviour i;
}

public class Spawner : MonoBehaviour {

    public SpawnInstance[] iS; //instanceS

    SpawnerPool[] sP; //spawnPool;

    int cK; //currentKey

    void Awake() {
        sP = new SpawnerPool[iS.Length];

        for (int i = 0; i < sP.Length; i++)
            sP[i] = new SpawnerPool(iS[i].n, CreateNewObject);

        //i = this;
        //Spawn("Projectile", new Vector3());
    }

    public PoolElement Spawn(string p) { //pool
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o.gameObject.SetActive(true);

        return iR;
        //(iR as OnSpawn).RunOnActive();
    }

    public PoolElement Spawn(string p, Vector3 l) { //pool, location
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o.gameObject.SetActive(true);
        iR.o.transform.position = l;

        Debug.Log("Working");
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public PoolElement Spawn(string p, Vector3 l, Transform t) { //pool, location, target
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o.gameObject.SetActive(true);

        iR.o.transform.parent = t;
        iR.o.transform.position = l;

        //(iR as OnSpawn).RunOnActive();
        Debug.Log(iR);
        return iR;
    }

    public void Remove(PoolElement i) {
        sP[BaseIteratorFunctions.IterateKey(sP, i.n)].sP.Store(i);
        i.o.gameObject.SetActive(false);
    }

    public void Remove(object[] p) {
        Remove(p[0] as PoolElement);
    }

    object CreateNewObject() {
        return new PoolElement(Instantiate(iS[cK].i), iS[cK].n);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerPool : BaseIterator {
    public Pool<PoolElement> sP;

    public SpawnerPool(string name, CI instanceCreator) {
        n = name;
        sP = new Pool<PoolElement>(instanceCreator, name);
    }
}

public class PoolElement : BaseIterator {
    public MonoBehaviour[] o; //object

    public PoolElement(MonoBehaviour[] obj, string name) {
        o = obj;
        n = name;
    }
}

[System.Serializable]
public class SpawnInstance : BaseIterator {
    public MonoBehaviour i;
}

[Serializable]
public class TypePool : BaseIterator {
    public TypeIterator[] t;//types;

    public TypePool(TypeIterator[] types, string name) {
        t = types;
        n = name;

        //for (int i = 0; i < types.Length; i++)
          //  t[i] = new TypeIterator(types[i], types[i].Name);
    }
}

[Serializable]
public class TypeIterator : BaseIterator {
    public Type t;
    public ObjectSettings s;//settings

    public TypeIterator(Type type) {
        t = type;
        n = type.Name;
    }

    public TypeIterator(Type type, ObjectSettings settings) {
        t = type;
        n = type.Name;

        s = settings;
    }
}

public class Modifier {
    public int v;//variable
    public object vV;//varaibleValue

    public Modifier(int value, object variableValue) {
        v = value;
        vV = variableValue;
    }
}

public class ObjectSettings {

    protected Modifier[] dV; //values

    public ObjectSettings() {
        dV = SetDefaultValues();
    }

    public void Reset(MonoBehaviour t) {
        SetObjectValues(t, dV);
    }

    public virtual Modifier[] SetDefaultValues() {
        return null;
    }

    public virtual void SetObjectValues(MonoBehaviour t,Modifier[] mods) {

    }
}

public class Spawner : MonoBehaviour {

    public TypePool[] tP; //typePool
    protected Type[] bB;  //baseBlocks

    SpawnerPool[] sP; //spawnPool;
    int cK; //currentKey

    void Awake() {
        CreateTypePool();

        sP = new SpawnerPool[tP.Length];

        for (int i = 0; i < sP.Length; i++)
            sP[i] = new SpawnerPool(tP[i].n, CreateNewObject);

        //i = this;
        //Spawn("Projectile", new Vector3());
    }

    public PoolElement ObjectRetriver(string p) {
        int cOK = BaseIteratorFunctions.IterateKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o[0].gameObject.SetActive(true);

        for (int i = 0; i < iR.o.Length; i++)
            if (tP[cK].t[i].s != null)
                tP[cK].t[i].s.Reset(iR.o[i]);

        return iR;
    }

    public virtual PoolElement Spawn(string p) { //pool
        return ObjectRetriver(p);
        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, float d) { //pool
        PoolElement iR = ObjectRetriver(p);

        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;
    }

    public virtual PoolElement Spawn(string p, Vector3 l) { //pool, location
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].gameObject.SetActive(true);
        iR.o[0].transform.position = l;

        Debug.Log("Working");
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, Vector3 l, float d) { //pool, location, duration
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].gameObject.SetActive(true);
        iR.o[0].transform.position = l;

        Debug.Log("Working");
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, Vector3 l, Transform t) { //pool, location, target
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].gameObject.SetActive(true);

        iR.o[0].transform.parent = t;
        iR.o[0].transform.position = l;

        //(iR as OnSpawn).RunOnActive();
        Debug.Log(iR);
        return iR;
    }

    public virtual void CreateTypePool() {
        //Just a handle for me to override and call to create TypePools.
    }

    public void Remove(PoolElement i) {
        sP[BaseIteratorFunctions.IterateKey(sP, i.n)].sP.Store(i);
        i.o[0].gameObject.SetActive(false);
    }

    public void Remove(object[] p) {
        Remove(p[0] as PoolElement);
    }

    object CreateNewObject() {
        GameObject newInstance = new GameObject(tP[cK].n, bB);

        MonoBehaviour[] scripts = new MonoBehaviour[tP[cK].t.Length];
        for (int i = 0; i < tP[cK].t.Length; i++)
            scripts[i] = newInstance.AddComponent(tP[cK].t[i].t) as MonoBehaviour;

        return new PoolElement(scripts, tP[cK].n);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerPool : Iterator
{
    public Pool<PoolElement> sP;

    public SpawnerPool(string name, CI instanceCreator)
    {
        n = name;
        sP = new Pool<PoolElement>(instanceCreator, name);
    }
}

public class ScriptIterator : Iterator
{
    public MonoBehaviour s;//script

    public ScriptIterator(string name, MonoBehaviour script)
    {
        n = name;
        s = script;
    }

}

public class PoolElement : Iterator
{
    public ScriptIterator[] o; //object

    public PoolElement(MonoBehaviour[] obj, string name)
    {
        n = name;
        o = new ScriptIterator[obj.Length];

        for (int i = 0; i < o.Length; i++)
            o[i] = new ScriptIterator(obj[i].GetType().Name, obj[i]);
    }
}

[System.Serializable]
public class SpawnInstance : Iterator
{
    public MonoBehaviour i;
}

[Serializable]
public class TypePool : Iterator
{
    public TypeIterator[] t;//types;

    public TypePool(TypeIterator[] types, string name)
    {
        t = types;
        n = name;
    }
}

[Serializable]
public class TypeIterator : Iterator
{
    public Type t;
    public PointerGroup pG; //pointerGroup

    public TypeIterator(Type type)
    {
        t = type;
        n = type.Name;
    }

    public TypeIterator(Type type, string pointerGroupName)
    {
        t = type;
        n = type.Name;
        pG = ReturnObject<PointerGroup>(PointerHolder.pL, pointerGroupName);
    }
}

/*public class Modifier
{
    public int v;//variable
    public object vV;//varaibleValue

    public Modifier(int value, object variableValue)
    {
        v = value;
        vV = variableValue;
    }
}
*/

/*public class ObjectSettings {

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
}*/

public class Spawner : MonoBehaviour
{

    public TypePool[] tP; //typePool
    protected Type[] bB;  //baseBlocks

    SpawnerPool[] sP; //spawnPool;
    int cK; //currentKey

    void Awake()
    {
        CreateTypePool();

        sP = new SpawnerPool[tP.Length];

        for (int i = 0; i < sP.Length; i++)
            sP[i] = new SpawnerPool(tP[i].n, CreateNewObject);

        //i = this;
        //Spawn("Projectile", new Vector3());
    }

    public virtual void CreateTypePool() //Just a handle for me to override and call to create TypePools.
    {
        
    }

    public PoolElement ObjectRetriver(string p)
    {
        int cOK = Iterator.ReturnKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o[0].s.gameObject.SetActive(true);

        for (int i = 0; i < iR.o.Length; i++)
            if (tP[cK].t[i].pG != null)
                for (int j = 0; j < tP[cK].t[i].pG.cP.Length; j++)
                    tP[cK].t[i].pG.cP[j].SetDefault(iR.o[i].s);


        return iR;
    }

    public virtual PoolElement Spawn(string p)
    { //pool
        return ObjectRetriver(p);
        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, float d)
    { //pool
        PoolElement iR = ObjectRetriver(p);

        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;
    }

    public virtual PoolElement Spawn(string p, Vector3 l)
    { //pool, location
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);
        iR.o[0].s.transform.position = l;

        Debug.Log("Working");
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, Vector3 l, float d)
    { //pool, location, duration
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);
        iR.o[0].s.transform.position = l;

        Debug.Log("Working");
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, Vector3 l, Transform t)
    { //pool, location, target
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);

        iR.o[0].s.transform.SetParent(t);
        iR.o[0].s.transform.position = l;

        //(iR as OnSpawn).RunOnActive();
        Debug.Log(iR);
        return iR;
    }

    public void SetPointer(PoolElement target, string component, string variableName, object value) //target, component, variableName, value
    {
        //Iterator.ReturnObject<PointerHolder>(,t.n)
        //Iterator.ReturnObject<ScriptIterator>(t.o, c).s;
        //t.n;
        MonoBehaviour script = Iterator.ReturnObject<ScriptIterator>(target.o, component).s;
        
        TypePool i0 = Iterator.ReturnObject<TypePool>(tP, target.n);

        TypeIterator i1 = Iterator.ReturnObject<TypeIterator>(i0.t, component);

        if (i1.pG == null)
        {
            Debug.LogErrorFormat("PointerGroup does not exist for {0}. Create one before trying to access it!", i1.n);
            return;
        }
        Debug.Log(script.GetType().Name);

        Iterator.ReturnObject<PointerHolder>(i1.pG.cP, variableName).Set(script,value);
    }

    public void Remove(PoolElement i)
    {
        Iterator.ReturnObject<SpawnerPool>(sP, i.n).sP.Store(i);
        i.o[0].s.gameObject.SetActive(false);
    }

    public void Remove(object[] p)
    {
        Remove(p[0] as PoolElement);
    }

    object CreateNewObject()
    {
        GameObject newInstance = new GameObject(tP[cK].n, bB);
        MonoBehaviour[] scripts = new MonoBehaviour[tP[cK].t.Length];

        for (int i = 0; i < tP[cK].t.Length; i++)
            scripts[i] = newInstance.AddComponent(tP[cK].t[i].t) as MonoBehaviour;

        return new PoolElement(scripts, tP[cK].n);
    }
}

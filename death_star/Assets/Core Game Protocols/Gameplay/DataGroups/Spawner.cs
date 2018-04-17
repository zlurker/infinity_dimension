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

public class TypeIterator<T> : TypeIterator
{
    public Action<T> dPS; //defaultParameterSetting

    public TypeIterator()
    {
        t = typeof(T);
    }

    public TypeIterator(Action<T> parameterSetting)
    {
        t = typeof(T);
        dPS = parameterSetting;
    }

    public override void SetDefault(object parameter)
    {
        if (dPS != null)
            dPS((T)parameter);
    }
}

[Serializable]
public class TypeIterator : Iterator
{
    public Type t;
    //public PointerGroup pG; //pointerGroup

    public TypeIterator()
    {
    }

    /*public TypeIterator(Type type)
    {
        t = type;
        n = type.Name;
    }*/

    /*public TypeIterator(Type type, string pointerGroupName)
        {
            t = type;
            n = type.Name;
            pG = ReturnObject<PointerGroup>(PointerHolder.pL, pointerGroupName);
        }*/

    public virtual void SetDefault(object parameter)
    {

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
    public static Spawner i;
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
            tP[cK].t[i].SetDefault(iR.o[i].s);//pG.cP[j].SetDefault(iR.o[i].s);

        return iR;
    }

    public virtual PoolElement Spawn(string p)
    { //pool
        return ObjectRetriver(p);
        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, DH d, object[] ps)
    {
        PoolElement inst = Spawn(p);
        SetVariable(inst, d, ps);
        return inst;
    }

    protected void SetVariable(PoolElement inst, DH d, object[] cP)
    {
        object[] fP = new object[cP.Length + 1];
        fP[0] = inst;

        for (int i = 1; i < fP.Length; i++)
            fP[i] = cP[i - 1];

        d.d(fP);
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

        return iR;

        //(iR as OnSpawn).RunOnActive();
    }

    public virtual PoolElement Spawn(string p, Vector3 l, float d)
    { //pool, location, duration
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);
        iR.o[0].s.transform.position = l;

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

        //Debug.Log(iR);
        return iR;
    }

    /*public object GetPointer(PoolElement target, string component, string variableName)
    {
        MonoBehaviour script = Iterator.ReturnObject<ScriptIterator>(target.o, component).s;
        return GetPointerLocation(target.n, component, variableName).Get(script);
    }*/

    /*public void SetPointer(PoolElement target, string component, string variableName, object value) //target, component, variableName, value
    {
        MonoBehaviour script = Iterator.ReturnObject<ScriptIterator>(target.o, component).s;
        Debug.Log(script.GetType().Name);

        GetPointerLocation(target.n, component, variableName).Set(script, value);
    }*/

    /*public PointerHolder GetPointerLocation(string targetName, string component, string variableName)
    {
        TypePool i0 = Iterator.ReturnObject<TypePool>(tP, targetName);
        TypeIterator i1 = Iterator.ReturnObject<TypeIterator>(i0.t, component);

        if (i1.pG == null)
        {
            Debug.LogErrorFormat("PointerGroup does not exist for {0}. Create one before trying to access it!", i1.n);
            return null;
        }

        return Iterator.ReturnObject<PointerHolder>(i1.pG.cP, variableName);
    }*/

    public static void Remove(PoolElement i)
    {
        Iterator.ReturnObject<SpawnerPool>(sP, i.n).sP.Store(i);
        i.o[0].s.gameObject.SetActive(false);
    }

    public void Remove(object[] p)
    {
        for (int i = 0; i < p.Length; i++)
            Remove(p[i] as PoolElement);
    }

    public static T GetCType<T>(PoolElement target) where T : class
    {
        for (int i = 0; i < target.o.Length; i++)
        {
            T inst = target.o[i].s as T;

            if (inst != null)
                return inst;
        }

        return null;
    }

    object CreateNewObject()
    {
        GameObject newInstance = new GameObject(tP[cK].n, bB);
        MonoBehaviour[] scripts = new MonoBehaviour[tP[cK].t.Length];

        for (int i = 0; i < tP[cK].t.Length; i++)
        {
            scripts[i] = newInstance.AddComponent(tP[cK].t[i].t) as MonoBehaviour;
            //Debug.Log(tP[cK].t[i].t.Name);
        }

        return new PoolElement(scripts, tP[cK].n);
    }
}

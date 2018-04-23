using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*public class SpawnerPool : Iterator
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

}*/

/*public class PoolElement : Iterator
{
    public ScriptIterator[] o; //object

    public PoolElement(MonoBehaviour[] obj, string name)
    {
        n = name;
        o = new ScriptIterator[obj.Length];

        for (int i = 0; i < o.Length; i++)
            o[i] = new ScriptIterator(obj[i].GetType().Name, obj[i]);
    }
}*/

/*[System.Serializable]
public class SpawnInstance : Iterator
{
    public MonoBehaviour i;
}*/

/*[Serializable]
public class TypePool : Iterator
{
    public TypeIterator[] t;//types;

    public TypePool(TypeIterator[] types, string name)
    {
        t = types;
        n = name;
    }
}
*/

public class ScriptableObject : MonoBehaviour
{
    public MonoBehaviour[] scripts;

    public void ResetScriptableObject()
    {
        scripts = new MonoBehaviour[0];
    }
}

public class ObjectCreationTemplate : Iterator
{
    public Func<ScriptableObject> d;

    public ObjectCreationTemplate(string name, Func<ScriptableObject> deleg)
    {
        n = name;
        d = deleg;
    }
}

public class TypeIterator<T> : TypeIterator where T : Component
{
    public Action<T> dPS; //defaultParameterSetting
    public Pool<T> tP; //typePool
    public Type[] oCM;// objCreatorMod
    public Spawner bBR; //baseBlockReturner

    public TypeIterator()
    {
        t = typeof(T);
        tP = new Pool<T>(CreateNewTypeObject, null);
    }

    /*public TypeIterator(Action<T> parameterSetting)
    {
        t = typeof(T);
        dPS = parameterSetting;
        tP = new Pool<T>(CreateNewTypeObject, null);
    }*/

    public TypeIterator(Spawner baseBlockReturner)
    {
        t = typeof(T);
        bBR = baseBlockReturner;
        tP = new Pool<T>(CreateNewTypeObject, null);
    }

    public TypeIterator(Action<T> paramterSetting, Spawner baseBlockReturner)
    {
        t = typeof(T);
        dPS = paramterSetting;
        bBR = baseBlockReturner;
        tP = new Pool<T>(CreateNewTypeObject, null);
    }

    public override void SetDefault(object p)
    {
        if (dPS != null)
            dPS((T)p);
    }

    public override object CreateNewTypeObject(object p)
    {
        return new GameObject(t.Name, bBR.GetBaseBlocks()).AddComponent<T>();
    }

    public override MonoBehaviour Retrive()
    {
        T inst = tP.Retrieve();
        SetDefault(inst);

        inst.gameObject.SetActive(true);
        return inst as MonoBehaviour;
    }

    public override void Remove(MonoBehaviour p)
    {
        p.gameObject.SetActive(false);
        tP.Store(p as T);
    }
}

public class ObjectDefaultSettings<T> : ObjectDefaultSettings
{
    public Action<T> sD; //settingDelegate

    public ObjectDefaultSettings(Action<T> settingDelegate)
    {
        sD = settingDelegate;
        t = typeof(T);
    }

    public override object ReturnDelegate()
    {
        return sD;
    }
}

public class ObjectDefaultSettings : Iterator
{
    public virtual object ReturnDelegate()
    {
        return null;
    }
}

[Serializable]
public class TypeIterator : Iterator
{
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

    public virtual void SetDefault(object p)
    {

    }

    public virtual object CreateNewTypeObject(object p)
    {
        return null;
    }

    public virtual MonoBehaviour Retrive()
    {
        return null;
    }

    public virtual void Remove(MonoBehaviour p)
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
    public Pool<ScriptableObject> bOL; //baseObjectList
    public Text test;
    public ObjectCreationTemplate[] tplts; //templates
    public ObjectDefaultSettings[] oDS; //objectDefaultSettings
    public List<TypeIterator> aTS;
    //public TypePool[] tP; //typePool
    protected Type[] bB;  //baseBlocks
    // SpawnerPool[] sP; //spawnPool;
    int cK; //currentKey

    public Spawner()
    {
        if (aTS == null)
            aTS = new List<TypeIterator>();

        bOL = new Pool<ScriptableObject>(CreateBaseObject, null);
        bB = new Type[0];

        CreateDefaultSettings();
        CreateTemplates();
    }

    void CreateTemplates()
    {
        tplts = new ObjectCreationTemplate[]{new ObjectCreationTemplate("ButtonMaker", () =>
        {
            ScriptableObject inst = UIDrawer.i.CreateScriptedObject(new MonoBehaviour[] { UIDrawer.i.CreateComponent<Image>(), UIDrawer.i.CreateComponent<Button>(), UIDrawer.i.CreateComponent<Text>() });
            GetCType<Button>(inst).targetGraphic = GetCType<Image>(inst);
            GetCType<Text>(inst).transform.SetParent(GetCType<Button>(inst).transform);
            GetCType<Image>(inst).transform.localScale = new Vector3(1, 0.3f);
            GetCType<Text>(inst).rectTransform.sizeDelta = new Vector2(100, 30);
            GetCType<Text>(inst).color = Color.black;
            return inst;
        }),
        new ObjectCreationTemplate("InputFieldMaker", () =>
            {
                ScriptableObject inst = UIDrawer.i.CreateScriptedObject(new MonoBehaviour[]{ UIDrawer.i.CreateComponent<Image>(), UIDrawer.i.CreateComponent<InputField>(),UIDrawer.i.CreateComponent<Text>() });
                GetCType<InputField>(inst).targetGraphic = GetCType<Image>(inst);
                GetCType<InputField>(inst).textComponent = GetCType<Text>(inst);

                GetCType<Text>(inst).color = Color.black;
                GetCType<Text>(inst).supportRichText = false;
                GetCType<Text>(inst).transform.SetParent(GetCType<InputField>(inst).transform);
                GetCType<Text>(inst).transform.position = Vector3.zero;

                GetCType<Text>(inst).transform.SetParent(GetCType<InputField>(inst).transform);
                //GetCType<Image>(inst).transform.SetParent(GetCType<InputField>(inst).transform);
                             
                return inst;
            })
        };
    }

    void CreateDefaultSettings()
    {
        oDS = new ObjectDefaultSettings[] {
            new ObjectDefaultSettings<Text>((t) =>{
                t.text = "DEFAULTWORDS";
                t.font = Resources.Load("jd-bold") as Font;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.horizontalOverflow = HorizontalWrapMode.Wrap;
                t.alignment = TextAnchor.MiddleCenter;
            })

        };
    }

    void Awake()
    {
        //CreateTypePool();

        //sP = new SpawnerPool[tP.Length];

        //aTS.Add(new TypeIterator<Text>());
        //aTS.Add(new TypeIterator<InputField>());
        //aTS.Add(new TypeIterator<InputField>());

        //CreateScriptedObject(new MonoBehaviour[] { CreateComponent<Text>() });
        //Remove(CreateNewUnit(new Type[] { typeof(Text) }));
        //CreateNewUnit(new Type[] { typeof(InputField) });
    }

    /*public virtual void CreateTypePool() //Just a handle for me to override and call to create TypePools.
    {

    }*/

    /*public PoolElement ObjectRetriver(string p)
    {
        int cOK = Iterator.ReturnKey(sP, p);
        cK = cOK;

        PoolElement iR = sP[cOK].sP.Retrieve();
        iR.o[0].s.gameObject.SetActive(true);

        for (int i = 0; i < iR.o.Length; i++)
            tP[cK].t[i].SetDefault(iR.o[i].s);//pG.cP[j].SetDefault(iR.o[i].s);

        return iR;
    }*/

    /*public virtual PoolElement Spawn(string p)
    { //pool
        return ObjectRetriver(p);
        //(iR as OnSpawn).RunOnActive();
    }*/

    /*public virtual PoolElement Spawn(string p, DH d, object[] ps)
    {
        PoolElement inst = Spawn(p);
        SetVariable(inst, d, ps);
        return inst;
    }*/

    /*protected void SetVariable(PoolElement inst, DH d, object[] cP) //Adds created pool element in as target, then set whatever variables it needs by delegate
     {
         object[] fP = new object[cP.Length + 1];
         fP[0] = inst;

         for (int i = 1; i < fP.Length; i++)
             fP[i] = cP[i - 1];

         d.d(fP);
     }*/

    /*public virtual PoolElement Spawn(string p, float d)
    { //pool
        PoolElement iR = ObjectRetriver(p);

        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;
    }*/

    /*public virtual PoolElement Spawn(string p, Vector3 l)
    { //pool, location
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);
        iR.o[0].s.transform.position = l;

        return iR;

        //(iR as OnSpawn).RunOnActive();
    }*/

    /*public virtual PoolElement Spawn(string p, Vector3 l, float d)
    { //pool, location, duration
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);
        iR.o[0].s.transform.position = l;

        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));
        return iR;

        //(iR as OnSpawn).RunOnActive();
    }*/

    /*public virtual PoolElement Spawn(string p, Vector3 l, Transform t)
    { //pool, location, target
        PoolElement iR = ObjectRetriver(p);

        iR.o[0].s.gameObject.SetActive(true);

        iR.o[0].s.transform.SetParent(t);
        iR.o[0].s.transform.position = l;

        //(iR as OnSpawn).RunOnActive();

        //Debug.Log(iR);
        return iR;
    }*/

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

    /*public void Remove(PoolElement i)
    {
        Iterator.ReturnObject<SpawnerPool>(sP, i.n).sP.Store(i);
        i.o[0].s.gameObject.SetActive(false);
    }*/

    /*public void Remove(object[] p)
    {
        for (int i = 0; i < p.Length; i++)
            Remove(p[i] as PoolElement);
    }*/

    /*public void Remove(MonoBehaviour target)
    {
        //Transform p = target
        for (int i = 0; i < aTS.Count; i++)
            if (aTS[i].t == target.GetType())
            {
                aTS[i].Remove(target);
                target.transform.parent = null;
                break;
            }
    }*/

    /* public void Remove(MonoBehaviour[] targets)
     {
         List<MonoBehaviour> t = new List<MonoBehaviour>(targets);

         for (int i = 0; i < aTS.Count; i++)
             for (int j = 0; j < targets.Length; j++)
                 if (aTS[i].t == targets[j].GetType())
                 {
                     aTS[i].Remove(targets[j]);
                     targets[j].transform.SetParent(null);
                     t.Remove(targets[j]);
                     break;
                 }
     }*/

    /*object CreateNewObject()
    {
        GameObject newInstance = new GameObject(tP[cK].n, bB);
        MonoBehaviour[] scripts = new MonoBehaviour[tP[cK].t.Length];

        for (int i = 0; i < tP[cK].t.Length; i++)
        {
            scripts[i] = newInstance.AddComponent(tP[cK].t[i].t) as MonoBehaviour;
            //Debug.Log(tP[cK].t[i].t.Name);
        }

        return new PoolElement(scripts, tP[cK].n);
    }*/

    /*public static T GetCType<T>(PoolElement target) where T : class
    {
        for (int i = 0; i < target.o.Length; i++)
        {
            T inst = target.o[i].s as T;

            if (inst != null)
                return inst;
        }

        return null;
    }*/

    /*public MonoBehaviour[] CreateNewUnit(object p)
    {
        List<Type> types = new List<Type>(p as Type[]); //Checks against my 
        List<MonoBehaviour> unitScripts = new List<MonoBehaviour>();
        GameObject baseObject = bOL.Retrieve();

        for (int i = 0; i < aTS.Count; i++)
            for (int j = 0; j < types.Count; j++)
                if (types[j] == aTS[i].t)
                {
                    MonoBehaviour inst = aTS[i].Retrive();
                    types.Remove(types[j]);
                    inst.transform.SetParent(baseObject.transform);
                    unitScripts.Add(inst);
                    break;
                }
        return unitScripts.ToArray();
    }*/

    public static T GetCType<T>(ScriptableObject target) where T : Component
    {
        for (int i = 0; i < target.scripts.Length; i++)
        {
            T inst = target.scripts[i] as T;

            if (inst != null)
                return inst;
        }

        return null;
    }

    public T CreateComponent<T>() where T : Component
    {
        TypeIterator inst = null;
        Action<T> pinst = null;
        ObjectDefaultSettings oinst = null;
        T toReturn;

        Debug.Log(aTS);
        inst = Iterator.ReturnObject<T>(aTS.ToArray()) as TypeIterator;
        oinst = Iterator.ReturnObject<T>(oDS) as ObjectDefaultSettings;

        if (oinst != null)
            pinst = oinst.ReturnDelegate() as Action<T>;

        if (inst == null)
            inst = new TypeIterator<T>(pinst, this);

        toReturn = inst.Retrive() as T;
        return toReturn;
    }

    public ScriptableObject CreateScriptedObject(MonoBehaviour[] scripts)
    {
        ScriptableObject baseObject = CustomiseBaseObject();
        baseObject.scripts = scripts;

        for (int i = 0; i < scripts.Length; i++)
            scripts[i].transform.SetParent(baseObject.transform);

        return baseObject;
    }

    public object CreateBaseObject(object p)
    {
        return new GameObject("ScriptBaseHolder").AddComponent<ScriptableObject>();
    }

    public virtual ScriptableObject CustomiseBaseObject()
    {
        return bOL.Retrieve();
    }

    public void Test()
    {
        Debug.Log(this);
    }

    public virtual Type[] GetBaseBlocks()
    {
        return bB;
    }
}

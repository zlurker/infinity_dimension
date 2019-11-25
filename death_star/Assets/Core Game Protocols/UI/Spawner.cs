using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScriptableObject : MonoBehaviour
{
    public MonoBehaviour[] scripts;
    public Spawner spawner;

    public void ResetScriptableObject()
    {
        scripts = new MonoBehaviour[0];
        transform.SetParent(null);
        gameObject.SetActive(false);
        spawner.bOL.Store(this);
    }
}

public class ScriptableObjectConstruction
{
    public List<MonoBehaviour> objects;

    public ScriptableObjectConstruction()
    {
        objects = new List<MonoBehaviour>();
    }

    public void AddMultiple(MonoBehaviour[] scripts)
    {
        for (int i = 0; i < scripts.Length; i++)
            objects.Add(scripts[i]);
    }
}

public class ObjectDefaultSettings<T> : ObjectDefaultSettings where T : Component
{
    public Action<T, ScriptableObjectConstruction> sD; //settingDelegate

    public ObjectDefaultSettings(Action<T, ScriptableObjectConstruction> settingDelegate)
    {
        sD = settingDelegate;
        t = typeof(T);
    }

    public override void RunDefaultSetting(MonoBehaviour target, ScriptableObjectConstruction sOC)
    {
        sD(target as T, sOC);
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

    public virtual void RunDefaultSetting(MonoBehaviour target, ScriptableObjectConstruction sOC)
    {
    }
}

public class TypeIterator<T> : TypeIterator where T : Component
{
    public Action<T, ScriptableObjectConstruction> dPS; //defaultParameterSetting
    public ObjectDefaultSettings oDS;
    public Pool<T> tP; //typePool
    public Spawner bBR; //baseBlockReturner

    public TypeIterator()
    {
        t = typeof(T);
        tP = new Pool<T>(CreateNewTypeObject, null);
    }

    public TypeIterator(Spawner baseBlockReturner)
    {
        t = typeof(T);
        bBR = baseBlockReturner;
        tP = new Pool<T>(CreateNewTypeObject, null);
        oDS = Iterator.ReturnObject<T>(baseBlockReturner.oDS) as ObjectDefaultSettings;
    }

    public TypeIterator(Action<T, ScriptableObjectConstruction> paramterSetting, Spawner baseBlockReturner)
    {
        t = typeof(T);
        dPS = paramterSetting;
        bBR = baseBlockReturner;
        tP = new Pool<T>(CreateNewTypeObject, null);
    }

    public override object CreateNewTypeObject(object p)
    {
        GameObject inst = new GameObject(t.Name, bBR.GetBaseBlocks());
        inst.SetActive(false);
        return inst.AddComponent<T>();
    }

    public override MonoBehaviour[] Retrive()
    {
        ScriptableObjectConstruction construct = new ScriptableObjectConstruction();
        T inst = tP.Retrieve();

        if (oDS != null)
            oDS.RunDefaultSetting(inst as MonoBehaviour, construct);

        inst.gameObject.SetActive(true);

        construct.objects.Add(inst as MonoBehaviour);
        return construct.objects.ToArray();
    }

    public override void Remove(MonoBehaviour p)
    {
        p.gameObject.SetActive(false);
        p.transform.SetParent(null);
        tP.Store(p as T);
    }
}

public class TypeIterator : Iterator
{
    public TypeIterator()
    {
    }

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

    public virtual MonoBehaviour[] Retrive()
    {
        return null;
    }

    public virtual void Remove(MonoBehaviour p)
    {
    }
}


public class Spawner : AbilityTreeNode, ISingleton
{
    public Pool<ScriptableObject> bOL; //baseObjectList
    public Pool<ScriptableObjectConstruction> sOC; //scriptableObjectConstruction
    public ObjectDefaultSettings[] oDS; //objectDefaultSettings
    public List<TypeIterator> aTS;
    public EnhancedList<ScriptableObject> sO; //spawnedObjects
    string name;
    //public static Spawner i;
    protected Type[] bB;  //baseBlocks

    public Spawner()
    {
        aTS = new List<TypeIterator>();

        bOL = new Pool<ScriptableObject>(CreateBaseObject, null);
        sO = new EnhancedList<ScriptableObject>();
        bB = new Type[0];
        name = GetType().Name;
        CreateDefaultSettings();
    }

    public virtual void CreateDefaultSettings()
    {
        oDS = new ObjectDefaultSettings[0];
    }

    public void Remove(ScriptableObject inst)
    {
        Debug.Log("Removing...");
        for (int i = 0; i < inst.scripts.Length; i++)
            (Iterator.ReturnObject(aTS.ToArray(), inst.scripts[i].GetType()) as TypeIterator).Remove(inst.scripts[i]);

        inst.ResetScriptableObject();
    }

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

    public static MonoBehaviour GetCType(ScriptableObject target,Type t) 
    {
        for (int i = 0; i < target.scripts.Length; i++)
            if (target.scripts[i].GetType() == t)
                return target.scripts[i];
        

        return null;
    }

    public static T GetCType<T>(ScriptableObjectConstruction target) where T : Component
    {
        for (int i = 0; i < target.objects.Count; i++)
        {
            T inst = target.objects[i] as T;

            if (inst != null)
                return inst;
        }

        return null;
    }


    public MonoBehaviour[] CreateComponent<T>() where T : Component
    {
        TypeIterator inst = null;

        inst = Iterator.ReturnObject<T>(aTS.ToArray()) as TypeIterator;

        if (inst == null)
        {
            inst = new TypeIterator<T>(this);
            aTS.Add(inst);
        }

        MonoBehaviour[] scripts = inst.Retrive();

        return scripts;
    }

    MonoBehaviour[] CreateComponent(Type t)
    {
        TypeIterator inst = null;

        inst = Iterator.ReturnObject(aTS.ToArray(), t) as TypeIterator;

        if (inst == null)
        {
            Type tInst = typeof(TypeIterator<>).MakeGenericType(t);
            inst = Activator.CreateInstance(tInst,new object[] { this }) as TypeIterator;
            aTS.Add(inst);
        }

        MonoBehaviour[] scripts = inst.Retrive();

        return scripts;
    }

    public ScriptableObject CalibrateScripts(ScriptableObject baseObject) {
        int id = sO.Add(baseObject);
        baseObject.gameObject.name = name + "-ScriptableObject" + id.ToString();

        for(int i = 0; i < baseObject.scripts.Length; i++) {
            if(baseObject.scripts[i].transform.parent == null)
                baseObject.scripts[i].transform.SetParent(baseObject.transform);

            baseObject.scripts[i].transform.localPosition = Vector3.zero;
            baseObject.scripts[i].gameObject.name = id.ToString();
        }

        return baseObject;
    }

    public virtual ScriptableObject CreateScriptedObject(Type[] type)
    {
        ScriptableObject baseObject = CustomiseBaseObject();
        MonoBehaviour[][] scripts = new MonoBehaviour[type.Length][];

        for (int i = 0; i < type.Length; i++)
            scripts[i] = CreateComponent(type[i]);

        baseObject.scripts = ConvertToSingleArray(scripts);
    
        return CalibrateScripts(baseObject);
    }

    public virtual ScriptableObject CreateScriptedObject(MonoBehaviour[][] scripts)
    {
        ScriptableObject baseObject = CustomiseBaseObject();
        baseObject.scripts = ConvertToSingleArray(scripts);
 
        return CalibrateScripts(baseObject);
    }

    public MonoBehaviour[] ConvertToSingleArray(MonoBehaviour[][] scripts)
    {
        List<MonoBehaviour> scriptsSL = new List<MonoBehaviour>();

        for (int i = 0; i < scripts.Length; i++)
            for (int j = 0; j < scripts[i].Length; j++)
                scriptsSL.Add(scripts[i][j]);

        return scriptsSL.ToArray();
    }

    public object CreateBaseObject(object p)
    {
        GameObject inst = new GameObject("ScriptBaseHolder", GetBaseBlocks());
        ScriptableObject sO = inst.AddComponent<ScriptableObject>();
        sO.spawner = this;

        sO.scripts = new MonoBehaviour[0];
        return sO;
    }

    public virtual ScriptableObject CustomiseBaseObject()
    {
        ScriptableObject bO = bOL.Retrieve();
        bO.gameObject.SetActive(true);
        return bO;
    }

    public Type[] GetBaseBlocks()
    {
        return bB;
    }

    public void RunOnStart()
    {

    }

    public void RunOnCreated()
    {
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI0","test1")
        };
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        NodeTaskingFinish();
        FireNode(0, VariableAction.SET);       
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScriptableObject : MonoBehaviour
{
    public MonoBehaviour[] scripts;

    public void ResetScriptableObject()
    {
        scripts = new MonoBehaviour[0];
        transform.SetParent(null);
        gameObject.SetActive(false);
        Spawner.i.bOL.Store(this);
    }
}

public class ScriptableObjectConstruction
{
    public List<MonoBehaviour> objects;

    public ScriptableObjectConstruction()
    {
        objects = new List<MonoBehaviour>();
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

    public virtual MonoBehaviour Retrive()
    {
        return null;
    }

    public virtual void Remove(MonoBehaviour p)
    {
    }
}

public class OnSpawnDelegates : Iterator
{
    public DH d;

    public OnSpawnDelegates(string name, DH deleg)
    {
        n = name;
        d = deleg;
    }
}

public struct DelegateInfo
{
    public string dN;//delegateNAme
    public object[] p; //parameters

    public DelegateInfo(string delegateName, object[] paramters)
    {
        dN = delegateName;
        p = paramters;
    }
}

public class Spawner : MonoBehaviour
{
    public Pool<ScriptableObject> bOL; //baseObjectList
    public Pool<ScriptableObjectConstruction> sOC; //scriptableObjectConstruction
    public ObjectCreationTemplate[] tplts; //templates
    public ObjectDefaultSettings[] oDS; //objectDefaultSettings
    public List<TypeIterator> aTS;
    public OnSpawnDelegates[] oSD; //onSpawnDelegates
    protected Type[] bB;  //baseBlocks
    public static Spawner i;

    public Spawner()
    {
        if (aTS == null)
            aTS = new List<TypeIterator>();

        if (i == null)
            i = this;

        bOL = new Pool<ScriptableObject>(CreateBaseObject, null);
        bB = new Type[0];

        CreateDefaultSettings();
        CreateTemplates();
        CreateOnSpawnDelegates();
    }

    void CreateTemplates()
    {
        tplts = new ObjectCreationTemplate[]{new ObjectCreationTemplate("ButtonMaker", () =>
        {
            ScriptableObject inst = UIDrawer.i.CreateScriptedObject(new MonoBehaviour[] { UIDrawer.i.CreateComponent<Image>(), UIDrawer.i.CreateComponent<Button>(), UIDrawer.i.CreateComponent<Text>() });
            GetCType<Button>(inst).targetGraphic = GetCType<Image>(inst);
            
            GetCType<Image>(inst).transform.localScale = new Vector3(1, 0.3f);

            GetCType<Text>(inst).transform.SetParent(GetCType<Button>(inst).transform);
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
            GetCType<Text>(inst).transform.SetParent(GetCType<InputField>(inst).transform);
            GetCType<Text>(inst).rectTransform.sizeDelta = new Vector2(100, 30);

            GetCType<Image>(inst).rectTransform.sizeDelta = new Vector2(100, 30);
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
                t.color = Color.white;
            }),

            new ObjectDefaultSettings<InputField>((t) =>{
                t.textComponent = null;
                t.text = "";
                t.onEndEdit.RemoveAllListeners();
                Debug.Log("Called");
            })
        };
    }

    void CreateOnSpawnDelegates()
    {
        oSD = new OnSpawnDelegates[]
        {
            new OnSpawnDelegates("Position",new DH((p) =>
            {
                ScriptableObject p0 = p[0] as ScriptableObject;
                Vector3 p1 = (Vector3) p[1];

                p0.transform.position = p1;
            })),
            new OnSpawnDelegates("UIPosition",new DH((p) =>
            {
                ScriptableObject p0 = p[0] as ScriptableObject;
                Vector3 p1 = (Vector3) p[1];

                p0.transform.position = UIDrawer.UINormalisedPosition(p1);
            }))
        };
    }

    protected void OnSpawn(string delegateName, ScriptableObject inst, object[] cP) //Adds created pool element in as target, then set whatever variables it needs by delegate
    {
        object[] fP = new object[cP.Length + 1];
        fP[0] = inst;

        for (int i = 1; i < fP.Length; i++)
            fP[i] = cP[i - 1];

        Iterator.ReturnObject<OnSpawnDelegates>(oSD, delegateName).d.d(fP);
    }

    public void Remove(ScriptableObject inst)
    {
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

    public T CreateComponent<T>() where T : Component
    {
        TypeIterator inst = null;
        Action<T> pinst = null;
        ObjectDefaultSettings oinst = null;
        T toReturn;

        inst = Iterator.ReturnObject<T>(aTS.ToArray()) as TypeIterator;
        oinst = Iterator.ReturnObject<T>(oDS) as ObjectDefaultSettings;

        if (oinst != null)
            pinst = oinst.ReturnDelegate() as Action<T>;

        if (inst == null)
        {
            inst = new TypeIterator<T>(pinst, this);
            aTS.Add(inst);
        }

        toReturn = inst.Retrive() as T;
        return toReturn;
    }

    public ScriptableObject CreateScriptedObject(MonoBehaviour[] scripts, DelegateInfo[] onSpawn = null)
    {
        ScriptableObject baseObject = CustomiseBaseObject();
        baseObject.scripts = scripts;

        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].transform.SetParent(baseObject.transform);
            scripts[i].transform.localPosition = Vector3.zero;
        }

        if (onSpawn != null)
            for (int i = 0; i < onSpawn.Length; i++)
                OnSpawn(onSpawn[i].dN, baseObject, onSpawn[i].p);

        return baseObject;
    }

    public object CreateBaseObject(object p)
    {
        return new GameObject("ScriptBaseHolder").AddComponent<ScriptableObject>();
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;


/// <summary>
/// Class which loads all singletons/editable classes.
/// </summary>
/// 

public enum ClassType {
    INTERFACE,BASECLASS

}
public class InterfaceLoader<T> : InterfaceLoader where T : class
{
    public T[] lI; //loadedInterfaces

    public InterfaceLoader(ClassType filter)
    {
        List<T> typeInstances = new List<T>();

        t = typeof(T);

        Type[] types = new Type[0];

        switch(filter) {

            case ClassType.INTERFACE:
                types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => t.IsAssignableFrom(p)).ToArray();
                break;

            case ClassType.BASECLASS:
                types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(t)).ToArray();
                break;
        }


        //lI = new T[types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);

            if (info != null)
                typeInstances.Add(info.Invoke(new object[0]) as T);
        }

        lI = typeInstances.ToArray();
    }

    public override object ReturnLoadedInterfaces()
    {
        return lI;
    }
}

public class InterfaceLoader : Iterator
{
    public virtual object ReturnLoadedInterfaces()
    {
        return null;
    }
}

public class LoadClasses : MonoBehaviour
{   
    void Start()
    {
        LoadedData.lI = new InterfaceLoader[] { new InterfaceLoader<ISingleton>(ClassType.INTERFACE), new InterfaceLoader<AbilityTreeNode>(ClassType.BASECLASS)};

        LoadSingletonClasses();

        //LoadEditableClasses();
        /*AbilityTreeNode[] interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];

        for (int i = 0; i < interfaces.Length; i++)
        {
            Debug.Log(interfaces[i].GetType());
        }*/

        SceneTransitionData.Initialise();
    }

    void LoadSingletonClasses()
    {
        ISingleton[] interfaces = (Iterator.ReturnObject<ISingleton>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as ISingleton[];
        LoadedData.sL = new Singleton[interfaces.Length];
        //Debug.Log(interfaces[0]);
        for (int i = 0; i < interfaces.Length; i++)
        {
            //Debug.Log(interfaces[i].GetType());
            MonoBehaviour singleton = new GameObject(interfaces[i].GetType().FullName).AddComponent(interfaces[i].GetType()) as MonoBehaviour;
            DontDestroyOnLoad(singleton.gameObject);
            (singleton as ISingleton).RunOnCreated();

            LoadedData.sL[i] = new Singleton(singleton as ISingleton);
        }

        /*List<ISingleton> singletonInstances = new List<ISingleton>();

        Type type = typeof(ISingleton);
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p)).ToArray();

        for (int i = 0; i < types.Length; i++)
        {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);

            if (info != null)
            {
                MonoBehaviour singleton = new GameObject(types[i].FullName).AddComponent(types[i]) as MonoBehaviour;
                (singleton as ISingleton).RunOnCreated();

                singletonInstances.Add(singleton as ISingleton);
            }
        }

        LoadedData.sL = singletonInstances.ToArray();*/
    }

    /*void LoadEditableClasses()
    {
        IPlayerEditable[] interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];

        List<IPlayerEditable> uiLoaderInstances = new List<IPlayerEditable>();
        List<IPlayerEditable> singletonInstances = new List<IPlayerEditable>();

        Type type = typeof(IPlayerEditable);
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p)).ToArray();

        for (int i = 0; i < types.Length; i++)
        {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);

            if (info != null)
            {
                IPlayerEditable createdInstance = info.Invoke(new object[0]) as IPlayerEditable;
                uiLoaderInstances.Add(createdInstance);
                singletonInstances.Add(null);

                if (createdInstance is ISingleton)
                    singletonInstances[singletonInstances.Count - 1] = (createdInstance as ISingleton).ReturnInstance() as IPlayerEditable;
            }
        }

        LoadedData.gIPEI = singletonInstances.ToArray();
        LoadedData.uL = uiLoaderInstances.ToArray();

        Debug.Log(LoadedData.uL.Length);
    }*/

    /*void LoadPointerData()
    {
        PointerHolder.pL = new PointerGroup[]
        {
            new PointerGroup("Text", new PointerHolder[]
            {
                new PointerHolder<Text,string>("text", (t,v) => { t.text = v; }, (t) => {return t.text; },"defaultvalue"),
                new PointerHolder<Text,int>("fontSize", (t,v) => { t.fontSize = v; },(t) => {return t.fontSize; }),
                new PointerHolder<Text,Font>("font", (t,v) => { t.font = v; },(t) =>  {return t.font; },Resources.Load("jd-bold"))
            }),

            new PointerGroup("Button", new PointerHolder[]
            {
                new PointerHolder<Button,DH>("listener", (t,v) => { t.onClick.AddListener(v.Invoke); }),
                //new PointerHolder<Text,int>((t,v) => { t.fontSize = v; }),
                //new PointerHolder<Text,Font>((t,v) => { t.font = v; },Resources.Load("jd-bold"))
            })
        };

        Debug.Log("Pointer Data Generated.");
    }*/
}

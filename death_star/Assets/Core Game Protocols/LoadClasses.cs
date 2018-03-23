using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

public class LoadClasses : MonoBehaviour
{

    void Start()
    {
        LoadPointerData();
        LoadSingletonClasses();
        LoadEditableClasses();

        SceneTransitionData.Initialise();
    }

    void LoadSingletonClasses()
    {
        List<ISingleton> singletonInstances = new List<ISingleton>();

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

        LoadedData.sL = singletonInstances.ToArray();
    }

    void LoadEditableClasses()
    {
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
    }

    void LoadPointerData()
    {
        PointerHolder.pL = new PointerGroup[]
        {
            new PointerGroup("Text", new PointerHolder[]
            {
                new PointerHolder<Text,string>("text", (t,v) => { t.text = v; },"defaultvalue"),
                new PointerHolder<Text,int>("fontSize", (t,v) => { t.fontSize = v; }),
                new PointerHolder<Text,Font>("font", (t,v) => { t.font = v; },Resources.Load("jd-bold"))
            }),

            new PointerGroup("Button", new PointerHolder[]
            {
                new PointerHolder<Button,DH>("listener", (t,v) => { t.onClick.AddListener(v.Invoke); }),
                //new PointerHolder<Text,int>((t,v) => { t.fontSize = v; }),
                //new PointerHolder<Text,Font>((t,v) => { t.font = v; },Resources.Load("jd-bold"))
            })
        };

        Debug.Log("Pointer Data Generated.");
    }
}

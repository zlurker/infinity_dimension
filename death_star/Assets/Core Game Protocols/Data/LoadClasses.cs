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
    INTERFACE, BASECLASS

}
public class InterfaceLoader<T> : InterfaceLoader where T : class {
    public T[] lI; //loadedInterfaces

    public InterfaceLoader(ClassType filter) {
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

        for(int i = 0; i < types.Length; i++) {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);

            if(info != null)
                typeInstances.Add(info.Invoke(new object[0]) as T);
        }

        lI = typeInstances.ToArray();
    }

    public override object ReturnLoadedInterfaces() {
        return lI;
    }
}

public class InterfaceLoader : Iterator {
    public virtual object ReturnLoadedInterfaces() {
        return null;
    }
}

public class LoadClasses : MonoBehaviour {
    void Start() {
        LoadedData.lI = new InterfaceLoader[] { new InterfaceLoader<ISingleton>(ClassType.INTERFACE), new InterfaceLoader<AbilityTreeNode>(ClassType.BASECLASS) };

        LoadSingletonClasses();
        LoadNetworkDependencies();

        SceneTransitionData.Initialise();
    }

    void LoadSingletonClasses() {
        ISingleton[] interfaces = (Iterator.ReturnObject<ISingleton>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as ISingleton[];
        LoadedData.sL = new Singleton[interfaces.Length];

        for(int i = 0; i < interfaces.Length; i++) {
            MonoBehaviour singleton = new GameObject(interfaces[i].GetType().FullName).AddComponent(interfaces[i].GetType()) as MonoBehaviour;
            DontDestroyOnLoad(singleton.gameObject);
            (singleton as ISingleton).RunOnCreated();

            LoadedData.sL[i] = new Singleton(singleton as ISingleton);
        }
    }

    void LoadNetworkDependencies() {
        // Creates a new instance, it will handle everything else in constructor.
        new NetworkObjectTracker();

        NetworkMessageEncoder.encoders = new List<NetworkMessageEncoder>();
        NetworkMessageEncoder.encoders.Add(new AbilityInputEncoder(0));
    }

}

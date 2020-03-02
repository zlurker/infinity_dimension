using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

public class LoadClasses : MonoBehaviour {

    void Start() {
        LoadSingletonClasses();
        LoadAbilityNodes();
        LoadNetworkDependencies();

        SceneTransitionData.Initialise();
    }

    void LoadAbilityNodes() {

        LoadedData.loadedNodeInstance = new Dictionary<Type, AbilityTreeNode>();

        Type[] types = new Type[0];
        Type t = typeof(AbilityTreeNode);

        types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(t)).ToArray();

        for(int i = 0; i < types.Length; i++) {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);
            AbilityTreeNode inst = null;

            if(info != null)
                inst = info.Invoke(new object[0]) as AbilityTreeNode;

            LoadedData.loadedNodeInstance.Add(types[i], inst);
        }
    }

    void LoadSingletonClasses() {

        LoadedData.singletonList = new Dictionary<Type, ISingleton>();

        Type[] types = new Type[0];
        Type t = typeof(ISingleton);

        types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => t.IsAssignableFrom(p)).ToArray();


        for(int i = 0; i < types.Length; i++) {

            ConstructorInfo info = types[i].GetConstructor(new Type[0]);
            ISingleton inst = null;

            if(info != null) {
                inst = info.Invoke(new object[0]) as ISingleton;

                MonoBehaviour singleton = new GameObject(inst.GetType().FullName).AddComponent(inst.GetType()) as MonoBehaviour;
                Debug.Log(singleton);
                DontDestroyOnLoad(singleton.gameObject);

                ISingleton castedSingleton = singleton as ISingleton;
                castedSingleton.RunOnCreated();

                LoadedData.singletonList.Add(types[i], castedSingleton);
            }
        }
    }

    void LoadNetworkDependencies() {
        // Creates a new instance, it will handle everything else in constructor.
        // to be replaced with igameplaystatic
        NetworkObjectTracker.inst = new NetworkObjectTracker();
        
        NetworkMessageEncoder.encoders = new NetworkMessageEncoder[] {
            new AbilityInputEncoder(),
            new UpdateAbilityDataEncoder()
        };

        for(int i = 0; i < NetworkMessageEncoder.encoders.Length; i++)
            NetworkMessageEncoder.encoders[i].SetEncoderId(i);
    }
}

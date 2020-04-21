﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LoadClasses : MonoBehaviour {

    void Start() {
        LoadSingletonClasses();
        LoadAbilityNodes();
        LoadNetworkDependencies();
        //AbilityValidator.ValidateAbilities();
    }

    void Update() {
        if(ClientProgram.clientId > -1 || ClientProgram.clientInst == null)
            SceneTransitionData.Initialise();
    }

    void LoadAbilityNodes() {

        LoadedData.loadedNodeInstance = new Dictionary<Type, AbilityTreeNode>();
        LoadedData.loadedParamInstances = new Dictionary<Type, LoadedRPWrapper>();

        Type[] types = new Type[0];
        Type t = typeof(AbilityTreeNode);

        types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(t)).ToArray();

        for(int i = 0; i < types.Length; i++) {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);
            AbilityTreeNode inst = null;

            if(info != null) {

                int linkedType = -1;

                Debug.Log(types[i] + " is linked to the following:");

                for(int j = 0; j < types.Length; j++) 
                    if(types[i].IsSubclassOf(types[j]) || types[j].IsSubclassOf(types[i])) {
                        Debug.Log(types[j]);
                    }
                

                //if(linkedType == -1)
                    //linkedType = typeList.Add(types[i]);

                inst = info.Invoke(new object[0]) as AbilityTreeNode;

                LoadedData.loadedNodeInstance.Add(types[i], inst);


                List<LoadedRuntimeParameters> nodeRp = new List<LoadedRuntimeParameters>();
                inst.GetRuntimeParameters(nodeRp);

                LoadedData.loadedParamInstances.Add(types[i], new LoadedRPWrapper(nodeRp.ToArray(), linkedType));
            }


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
            new ServerChannel(),
            //new AbilityInputEncoder(),
            new UpdateAbilityDataEncoder(),
            new PlayerCustomDataTrasmitter(),
            new ImageDependenciesTransfer(),
            new PlayerCharacterCreationEncoder(),
            new ManifestEncoder()
        };

        for(int i = 0; i < NetworkMessageEncoder.encoders.Length; i++)
            NetworkMessageEncoder.encoders[i].CalibrateEncoder(i);
    }
}

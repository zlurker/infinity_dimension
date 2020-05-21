using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class LoadClasses : MonoBehaviour {

    void Start() {
        LoadedData.gameDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pixel");

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
        LoadedData.loadedBuiltInheritances = new Dictionary<Type, HashSet<Type>>();

        Type[] types = new Type[0];
        Type rootSubclass = typeof(AbilityTreeNode);

        types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(rootSubclass)).ToArray();



        for(int i = 0; i < types.Length; i++) {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);
            AbilityTreeNode inst = null;

            if(info != null) {

                // Builds inheritance.
                IEnumerable<Type> subclasseses = types.Where(t => t.IsSubclassOf(types[i]));
                LoadedData.loadedBuiltInheritances.Add(types[i], new HashSet<Type>(subclasseses));

                inst = info.Invoke(new object[0]) as AbilityTreeNode;

                LoadedData.loadedNodeInstance.Add(types[i], inst);


                List<LoadedRuntimeParameters> nodeRp = new List<LoadedRuntimeParameters>();
                inst.GetRuntimeParameters(nodeRp);

                LoadedData.loadedParamInstances.Add(types[i], new LoadedRPWrapper(nodeRp.ToArray()));
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
        NetworkMessageEncoder.encoders = new NetworkMessageEncoder[] {
            new ServerChannel(),
            //new AbilityInputEncoder(),
            new UpdateAbilityDataEncoder(),
            new PlayerCustomDataTrasmitter(),
            new ImageDependenciesTransfer(),
            new PlayerCharacterCreationEncoder(),
            new ManifestEncoder(),
            new InputSignalEncoder()
        };

        for(int i = 0; i < NetworkMessageEncoder.encoders.Length; i++)
            NetworkMessageEncoder.encoders[i].CalibrateEncoder(i);
    }
}

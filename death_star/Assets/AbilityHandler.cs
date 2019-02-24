using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClassFilter<T> : ClassFilter { //Unless i use this as the parameter.
    public Func<T, IPlayerEditable> gI;


    public ClassFilter(Type type, Func<T, IPlayerEditable> getInstance) {
        t = type;
        gI = getInstance;
    }
}

public class ClassFilter {
    public Type t;
}

public class GenericTrigger {
    public static IPlayerEditable GenericMethod<T>(T parameter, Type type) {
        IPlayerEditable instance = null;

        ClassFilter filter = Iterator.ReturnObject(PresetGameplayData.standaloneClass, (t) => {
            return t.t.IsAssignableFrom(type);
        });

        if(filter != null)
            instance = (filter as ClassFilter<T>).gI(parameter);

        return instance;
    }
}

public class AbilityHandler : MonoBehaviour {

    public string path;
    SavedData[] loadedData;

    void Start() {
        loadedData = SavedData.CreateLoadFile(path);
        Singleton.GetSingleton<PlayerInput>().AddNewInput(KeyCode.Alpha1, new DH(TriggerAbility, new object[] { 0 }), 0);
    }

    void TriggerAbility(object[] p) {
        int abilityId = (int)p[0];

        for(int i = 0; i < loadedData.Length; i++) {
            if(loadedData[i].connectedInt < 0) {

                IPlayerEditable inst = GenericTrigger.GenericMethod(loadedData[i].classType, loadedData[i].classType);
                if(inst != null)
                    inst.SetValues(loadedData[i].fields.ToArray());
            }
        }
    }
}


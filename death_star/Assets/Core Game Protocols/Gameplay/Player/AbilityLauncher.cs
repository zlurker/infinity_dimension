using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Output {
    public List<ScriptableObject> spawnedObjects; //Outputs created objects

    public Output() {
        spawnedObjects = new List<ScriptableObject>();
    }
}

public class AbilityLauncher {
    public string n;
    public Type[] c;

    public AbilityLauncher(string name, Type[] component) {
        n = name;
        c = component;
    }

    public Output RunAbility() {
        Output o = new Output();

        for(int i = 0; i < c.Length; i++)
            Iterator.ReturnObject(LaunchPad.lPS as LaunchPad[], (t0) => {
                return t0.t.IsAssignableFrom(c[i]);
            }).RunMethod(c[i], o);

        return o;
    }
}

public class LaunchPad : Iterator {
    public static Iterator[] lPS = new LaunchPad[] {
        new LaunchPad(typeof(ISingleton),(t,o)=> {
            //(Singleton.GetSingleton(t) as IPlayerEditable)
        }),
        new LaunchPad(typeof(ISpawnable),(t,o)=> {
            ScriptableObject sO = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[]{ t});
            Spawner.GetCType(sO,t);
        })
    };

    Action<Type, Output> m;

    public LaunchPad(Type type, Action<Type, Output> method) {
        t = type;
        m = method;
    }

    public void RunMethod(Type instance, Output toOut) {
        m(instance, toOut);
    }
}


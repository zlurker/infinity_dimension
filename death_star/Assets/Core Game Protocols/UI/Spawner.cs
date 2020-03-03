using System.Collections.Generic;
using UnityEngine;
using System;

public interface IOnSpawn {
    void OnSpawn();
}

public class SpawnerOutput {
    public MonoBehaviour script;
    public Type scriptType;


    public SpawnerOutput(MonoBehaviour s, Type t) {
        script = s;
        scriptType = t;
    }
}

public class Spawner : MonoBehaviour, ISingleton {

    public static Dictionary<Type, List<MonoBehaviour>> typePool = new Dictionary<Type, List<MonoBehaviour>>();
    protected Type[] bB; 

    public void Remove(SpawnerOutput inst) {
        //Debug.Log("Removing...");
        if(!typePool.ContainsKey(inst.scriptType))
            typePool.Add(inst.scriptType, new List<MonoBehaviour>());

        typePool[inst.scriptType].Add(inst.script);
    }

    public virtual SpawnerOutput CreateScriptedObject(Type type) {

        if(!typePool.ContainsKey(type)) 
            typePool.Add(type, new List<MonoBehaviour>());

        MonoBehaviour inst = null;

        if(typePool[type].Count > 0) {
            inst = typePool[type][0];
            typePool[type].RemoveAt(0);
        } else 
            inst = new GameObject(type.Name, bB).AddComponent(type) as MonoBehaviour;            
        

        IOnSpawn onSpawn = inst as IOnSpawn;

        if(onSpawn != null)
            onSpawn.OnSpawn();

        return new SpawnerOutput(inst,type);
    }

    public void RunOnStart() {
    }

    public void RunOnCreated() {
        bB = new Type[0];
    }

    /*public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<GameObject>("Output Object",null)
        };
    }*/
}

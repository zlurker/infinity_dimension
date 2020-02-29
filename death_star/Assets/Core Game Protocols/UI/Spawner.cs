using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public struct SpawnerOutput {
    public MonoBehaviour script;
    public Type scriptType;

    // More for reference on deleting.
    public SpawnerOutput[] additionalScripts;

    public SpawnerOutput(MonoBehaviour s, Type t) {
        script = s;
        scriptType = t;
        additionalScripts = null;
    }

    public SpawnerOutput(MonoBehaviour s, Type t, SpawnerOutput[] aS) {
        script = s;
        scriptType = t;
        additionalScripts = aS;
    }
}

public class Spawner : AbilityTreeNode, ISingleton {

    public static Dictionary<Type, List<MonoBehaviour>> typePool = new Dictionary<Type, List<MonoBehaviour>>();
    protected Type[] bB; 

    public void Remove(SpawnerOutput inst) {
        //Debug.Log("Removing...");
        if(!typePool.ContainsKey(inst.scriptType))
            typePool.Add(inst.scriptType, new List<MonoBehaviour>());

        typePool[inst.scriptType].Add(inst.script);

        if(inst.additionalScripts != null)
            for(int i = 0; i < inst.additionalScripts.Length; i++)
                Remove(inst.additionalScripts[i]);
    }

    public SpawnerOutput CreateScriptedObject(Type type) {

        if(!typePool.ContainsKey(type)) 
            typePool.Add(type, new List<MonoBehaviour>());

        MonoBehaviour inst = null;

        if (typePool[type].Count > 0) {
            inst = typePool[type][0];
            typePool[type].RemoveAt(0);
        } else 
            inst = new GameObject(type.Name,bB).AddComponent(type) as MonoBehaviour;

        return new SpawnerOutput(inst,type);
    }

    public void RunOnStart() {
    }

    public void RunOnCreated() {
        bB = new Type[0];
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<GameObject>("Output Object",null)
        };
    }
}

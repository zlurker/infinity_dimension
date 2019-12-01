using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MAssSpawnTest : MonoBehaviour {

    // Use this for initialization
    int i = 0;
    bool test = false;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(!test)
            for(i = 0; i < 100; i++)
                Singleton.GetSingleton<Spawner>().CreateScriptedObject(new Type[] { });

        test = true;
    }
}

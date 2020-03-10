using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : AbilityTreeNode {

    bool test;
    // Use this for initialization
    void Start() {

    }

    void Update() {
        if(!test) {
            int value = Random.Range(100, 400);
            //SyncDataWithNetwork(0, value);
            Debug.Log(value);
            test = true;
        }

    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Random Int", 0)
        };
    }
}

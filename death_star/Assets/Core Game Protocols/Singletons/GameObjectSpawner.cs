using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class GameObjectSpawner : Spawner
{

    public static GameObjectSpawner i; //instance

    // Use this for initialization
    void Start()
    {
        i = this;
        //Spawn("Projectile", new Vector3());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Name of Projectile","test"),
            new RuntimeParameters<float>("Projectile Speed", 1),
            new RuntimeParameters<float>("Projectile Damage", 2),
            new RuntimeParameters<int>("Multiplier", 3),
        };
    }

    public void Invoke(Iterator[] parameters) {
        throw new System.NotImplementedException();
    }

    public void SetValues(RuntimeParameters[] runtimeParameters) {
        Debug.Log("GOS Called");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class GameObjectSpawner : Spawner, IPlayerEditable
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

    public MethodInfo GetMainMethod()
    {

        return null;
        //UIDrawer.i.Spawn("Image");
    }

    public RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Name of Projectile","Marcus Warts"),
            new RuntimeParameters<float>("Projectile Speed", 5),
            new RuntimeParameters<float>("Projectile Damage", 20),
            new RuntimeParameters<int>("Multiplier", 15)
            
        };
    }

    public void Invoke(Iterator[] parameters) {
        throw new System.NotImplementedException();
    }
}

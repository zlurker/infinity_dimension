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

    public RuntimeParameters[] GetRuntimeParameters() {
        SavedData projectile = new SavedData(typeof(Projectile));
        SavedData amazingshit = new SavedData(typeof(UIDrawer));

        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Name of Projectile","Marcus Warts"),
            new RuntimeParameters<float>("Projectile Speed", 5),
            new RuntimeParameters<float>("Projectile Damage", 20),
            new RuntimeParameters<int>("Multiplier", 15),
            new RuntimeParameters<EditableLinkInstance>("On Hit",new EditableLinkInstance(new SavedData[]{projectile,amazingshit }))     
        };
    }

    public void Invoke(Iterator[] parameters) {
        throw new System.NotImplementedException();
    }

    public void SetValues(RuntimeParameters[] runtimeParameters) {
        Debug.Log("GOS Called");
    }
}

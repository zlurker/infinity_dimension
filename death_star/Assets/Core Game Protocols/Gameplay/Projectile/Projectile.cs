using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Projectile : MonoBehaviour,ISpawnable,IPlayerEditable
{
    public RuntimeParameters[] parameters;
  
    void Start()
    {

    }

    void Update()
    {
        transform.parent.position += new Vector3(1, Mathf.Tan(70 * Mathf.Deg2Rad)) * Iterator.ReturnObject<RuntimeParameters<float>>(parameters, "Projectile Speed").v;
    }

    public void OnSpawn()
    {
       
    }

    public RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Name of Projectile","Marcus Warts"),
            new RuntimeParameters<float>("Projectile Speed", 5),
            new RuntimeParameters<float>("Projectile Damage", 20),
            new RuntimeParameters<int>("Multiplier", 15),
            new RuntimeParameters<EditableLinkInstance>("On Hit",new EditableLinkInstance(new SavedData[0]))
        };
    }

    public void SetValues(RuntimeParameters[] values) {
        parameters = values; 
    }
}

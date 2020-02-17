using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Projectile : AbilityTreeNode,ISpawnable
{
    public RuntimeParameters[] parameters;
  
    void Start()
    {

    }

    void Update()
    {
        //transform.parent.position += new Vector3(1, Mathf.Tan(70 * Mathf.Deg2Rad)) * Iterator.ReturnObject<RuntimeParameters<float>>(parameters, "Projectile Speed").v;
    }

    public void OnSpawn()
    {
       
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<int>("Speed", 5)
        };
    }

    public void SetValues(RuntimeParameters[] values) {
        parameters = values; 
    }

    public override void ThreadEndStartCallback(int threadId) {
        Debug.Log("Ultimate callback by thread " + threadId);
    }
}

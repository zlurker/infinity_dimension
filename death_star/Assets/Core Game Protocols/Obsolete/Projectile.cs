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
            new RuntimeParameters<string>("Name of Projectile","Marcus Warts"),
            new RuntimeParameters<float>("Projectile Speed", 5),
            new RuntimeParameters<float>("Projectile Damage", 20),
            new RuntimeParameters<int>("Multiplier", 15),
        };
    }

    public void SetValues(RuntimeParameters[] values) {
        parameters = values; 
    }

    public override void NodeCallback(int nId, int variableCalled, VariableAction action) {
        Debug.Log("0, Callback was called by:" + nId);
        Debug.Log((GetTransverserObject().GetVariable(nId)[variableCalled].field as RuntimeParameters<string>).v);
        NodeTaskingFinish();
    }
}

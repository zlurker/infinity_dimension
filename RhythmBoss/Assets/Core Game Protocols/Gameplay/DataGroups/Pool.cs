using System;
using System.Collections.Generic;
using UnityEngine;

public delegate object CI(); //Create Instance //Peformance might be expensive. Need to find out another way to point to functions to create instance. 

public class Pool<T> {

    public Stack<T> pool = new Stack<T>();
    CI iC;
    int cI = 0;
    string n = "";

    public Pool(CI instanceCreator) {
        iC = instanceCreator;
    }

    public Pool(CI instanceCreator, string debug) {
        iC = instanceCreator;
        n = debug;
    }

    public T Retrieve() {
        if (pool.Count == 0) {
            pool.Push((T)iC());
            cI++;
        }

        if (n.Length != 0)
            Debug.LogFormat("Current_Number_Of_{0}_Instances: {1}", n, cI);

        return pool.Pop();
    }

    public void Store(T inst) {
        pool.Push(inst);
    }
}



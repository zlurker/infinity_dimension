using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListNullTest : MonoBehaviour {

    List<Spawner> test;
	// Use this for initialization
	void Start () {
        Debug.Log(test);

        DoSomething();
        test = new List<Spawner>(new Spawner[7]);

        for (int i=0; i < test.Count; i++)
        {
            Debug.Log(i);
            test.Remove(test[i]);
            Debug.Log(i);
        }

        DoSomething();
        Debug.Log(test);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DoSomething()
    {
        if (test == null)
            Debug.Log("LIst is null");
        else
            Debug.Log("Its not");
    }
}

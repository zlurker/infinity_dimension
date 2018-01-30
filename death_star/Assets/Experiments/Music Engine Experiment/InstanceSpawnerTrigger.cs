using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceSpawnerTrigger : MonoBehaviour {

    List<PoolElement> t; 

    void Start() {
        t = new List<PoolElement>();
        Loop(new object[0]);

    }

	// Update is called once per frame
	void Update () {
		
	}

    void Loop(object[] test) {
        
        t.Add(GameObjectSpawner.i.Spawn("TestObj"));

        PatternControl.i.Pattern_Args("VECTOR_PATTERN,GROUP_PATTERN", t.ToArray(), new object[][] { new object[] { new Vector3(0,0), new Vector3(1,1)}, new object[] {"TEST" } });
        GroupElement testreturn = PatternControl.i.Get_Group("TEST");

        Debug.Log(testreturn.n);
        for (int i = 0; i < testreturn.gE.Count; i++)
            Debug.Log(testreturn.gE[i].o.transform.position);
        
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + 1, new DH(Loop)));
    }
}

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
    void Update() {

    }

    void Loop(object[] test) {

        t.Add(GameObjectSpawner.i.Spawn("TestObj"));

        PatternControl.i.Pattern_Args(t.ToArray(),
            new object[][] {
                new object[] { "VECTOR_PATTERN", new Vector3(0,0), new Vector3(1,1)},
                new object[] { "GROUP_PATTERN","TEST", "ADD_PARAMETER_OBJECTS,PARENT_PARAMETER_OBJECTS" }
            });

        object[] testreturn = PatternControl.i.Pattern_Args(null,
            new object[][] { new object[] { "GROUP_PATTERN","TEST", "GET_GROUP" } }
            );

        GroupElement te = testreturn[0] as GroupElement;

        Debug.Log(te.n);
        for (int i = 0; i < te.gE.Count; i++)
            Debug.Log(te.gE[i].o[0].transform.position);

        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + 1, new DH(Loop)));
    }
}

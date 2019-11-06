using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Obsolete("Not used any more", true)]
public class LineUpdater  {
    public string gN;

    public LineUpdater(string groupName) {
        gN = groupName;
    }

    public void OnDrag() {
        Group lines = Singleton.GetSingleton<PatternControl>().GetGroup(gN);
        //Debug.Log(gN);
        for(int i = 0; i < lines.gE.Count; i++)
            Spawner.GetCType<Line>(lines.gE[i]).EstablishJoint();
    }
}


public class Line : MonoBehaviour {

    public Transform target;
    public Transform lineRoot;
    public ScriptableObject sO;

    public void EstablishJoint() {
        EstablishJoint(target.position);
    }

    public void EstablishJoint(Vector3 oppEnd) {
        if(lineRoot != null)
            transform.parent.position = lineRoot.position;

        if(sO == null)
            sO = Singleton.GetSingleton<UIDrawer>().sO.l[int.Parse(gameObject.name)];

        
    }
}

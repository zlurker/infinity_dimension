using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineUpdater : IWindowsDragEvent {
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

        Vector2 d = oppEnd - transform.parent.position;
        Spawner.GetCType<Image>(sO).rectTransform.sizeDelta = new Vector2(10f, d.magnitude);
        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(oppEnd - transform.parent.position)));
    }
}

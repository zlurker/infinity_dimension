using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    public Transform target;
    public Transform lineRoot;

    public void EstablishJoint() {
        EstablishJoint(target.position);
    }

    public void EstablishJoint(Vector3 oppEnd) {
        if(lineRoot != null)
            transform.parent.position = lineRoot.position;

        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(oppEnd - transform.parent.position)));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILocation : BaseIterator {
    public RectTransform p; //position

    public UILocation(RectTransform position, string name) {
        p = position;
        n = name;
    }
}

public class UIData : MonoBehaviour {
    UILocation[] pD;//pointData
    public Canvas t; // target
    public static UIData i;

    void Awake() {
        i = this;
        pD = new UILocation[transform.childCount];

        for (int i=0; i < transform.childCount; i++) {
            Transform c = transform.GetChild(i);
            pD[i] = new UILocation(c as RectTransform, c.name);
        }        
    }

    public Vector3 ReturnPoint(string name) {
        return pD[BaseIteratorFunctions.IterateKey(pD, name)].p.position;
    }
}

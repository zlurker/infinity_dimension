using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPosPivotTest : MonoBehaviour {

    public RectTransform obj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        obj.SetParent(transform as RectTransform);
        obj.localPosition = new Vector3(obj.pivot.x * obj.sizeDelta.x, -obj.pivot.y * obj.sizeDelta.y);
	}
}

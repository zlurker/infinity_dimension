using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigonometry : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(1, Mathf.Tan(70 * Mathf.Deg2Rad));
        Debug.Log(Mathf.Tan(70 * Mathf.Deg2Rad));
	}
}

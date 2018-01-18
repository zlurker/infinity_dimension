using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UITesting : MonoBehaviour {

    public Button test;
    public Text aas;
    public Image asd;
    public MonoBehaviour t;
    public string[] test2;

    void Start () {
        //Debug.Log(test2.Min(w => w.Length));
        //test.onClick.AddListener(() =>Test(999));
        test.image = null;
        aas.text = null;
        asd.sprite = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Test(int i) {
        Debug.LogFormat("Working {0}",i);
    }
}

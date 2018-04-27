using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldBug : MonoBehaviour {

    public GameObject test;
    public Text testText;
    InputField testComponent;
	// Use this for initialization
	void Start () {
        testComponent = test.AddComponent<InputField>();
        testComponent.textComponent = testText;
        testComponent.textComponent = null;

        test.gameObject.SetActive(false);        
        test.gameObject.SetActive(true);

        testComponent.textComponent = testText;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

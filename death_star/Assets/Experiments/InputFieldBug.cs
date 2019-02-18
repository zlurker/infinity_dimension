using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldBug : MonoBehaviour {

    public GameObject test;
    public Text testText;
    public InputField testComponent;
    public Text text;
    public Image image;
	// Use this for initialization
	void Start () {
        testComponent.gameObject.SetActive(false);

        testComponent.textComponent = null;
        testComponent.text = "";
        testComponent.onEndEdit.RemoveAllListeners();

        text.color = Color.black;
        text.supportRichText = false;
        text.transform.SetParent(testComponent.transform);
        text.rectTransform.sizeDelta = new Vector2(100, 30);

        image.rectTransform.sizeDelta = new Vector2(100, 30);
        testComponent.gameObject.SetActive(true);
        testComponent.textComponent = text;
        testComponent.targetGraphic = image;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

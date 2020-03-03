using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UIWrapperBase : MonoBehaviour, IOnSpawn {

    // Main script in this. 
    public MonoBehaviour mainScript;
    // Additional support scripts.
    public SpawnerOutput[] additionalScripts;

    public virtual void OnSpawn() {

    }

    public void AllignWrapperElements() {
        if(additionalScripts != null)
            for(int i = 0; i < additionalScripts.Length; i++) {
                additionalScripts[i].script.transform.SetParent(transform);
                additionalScripts[i].script.transform.localPosition = Vector3.zero;
            }
    }
}

[RequireComponent(typeof(Text))]
public class TextWrapper : UIWrapperBase {

    public override void OnSpawn() {
        if (mainScript == null)
            mainScript = GetComponent<Text>();

        Text t = mainScript as Text;

        t.text = "DEFAULTWORDS";
        t.font = Resources.Load("jd-bold") as Font;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.black;
        (t.transform as RectTransform).sizeDelta = new Vector2(100, 20);
    }
}

[RequireComponent(typeof(Button))]
public class ButtonWrapper : UIWrapperBase {

    public override void OnSpawn() {

        if (mainScript == null)
            mainScript = GetComponent<Button>();

        Button b = mainScript as Button;

        b.onClick.RemoveAllListeners();

        additionalScripts = new SpawnerOutput[] {
                LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
                LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper))
            };

        Image i = additionalScripts[0].script as Image;
        Text t = (additionalScripts[1].script as TextWrapper).mainScript as Text;

        b.targetGraphic = i;

        i.rectTransform.sizeDelta = new Vector3(100, 30);
        (b.transform as RectTransform).sizeDelta = new Vector3(100, 30);
   
        t.rectTransform.sizeDelta = new Vector2(100, 30);
        t.color = Color.black;
        AllignWrapperElements();
    }
}

[RequireComponent(typeof(InputField))]
public class InputFieldWrapper : UIWrapperBase {

    public override void OnSpawn() {

        if (mainScript == null)
            mainScript = GetComponent<InputField>();

        InputField iF = mainScript as InputField;

        iF.textComponent = null;
        iF.text = "";
        iF.onEndEdit.RemoveAllListeners();

        additionalScripts = new SpawnerOutput[] {
                LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
                LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper))
            };

        Image i = additionalScripts[0].script as Image;
        Text t = (additionalScripts[1].script as TextWrapper).mainScript as Text;

        iF.gameObject.SetActive(true);

        iF.textComponent = t;
        iF.targetGraphic = i;
        (iF.transform as RectTransform).sizeDelta = new Vector2(100, 30);

        t.color = Color.black;
        t.supportRichText = false;

        t.rectTransform.sizeDelta = new Vector2(100, 30);
        i.rectTransform.sizeDelta = new Vector2(100, 30);
        AllignWrapperElements();
    }
}


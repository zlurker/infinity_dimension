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

    /*public void AllignWrapperElements() {
        if(additionalScripts != null)
            for(int i = 0; i < additionalScripts.Length; i++) {
                additionalScripts[i].script.transform.SetParent(transform);
                additionalScripts[i].script.transform.localPosition = Vector3.zero;
            }
    }*/

    /*public void PopulateScriptDirectory(Dictionary<string,int> targetDict) {
        for(int i = 0; i < additionalScripts.Length; i++)
            targetDict.Add(additionalScripts[i].scriptName,i); 
    }*/
}

[RequireComponent(typeof(Text))]
public class TextWrapper : UIWrapperBase {

    public Text text;

    public override void OnSpawn() {
        if(mainScript == null)
            mainScript = GetComponent<Text>();

        text = mainScript as Text;

        text.text = "DEFAULTWORDS";
        text.font = Resources.Load("jd-bold") as Font;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        (text.transform as RectTransform).sizeDelta = new Vector2(100, 20);
    }
}

[RequireComponent(typeof(Button))]
public class ButtonWrapper : UIWrapperBase {

    public Button button;
    public Image image;
    public Text text;

    public override void OnSpawn() {

        if(mainScript == null)
            mainScript = GetComponent<Button>();

        button = mainScript as Button;

        button.onClick.RemoveAllListeners();

        additionalScripts = new SpawnerOutput[] {
                 LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
                LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper))
            };

        image = additionalScripts[0].script as Image;
        text = (additionalScripts[1].script as TextWrapper).mainScript as Text;

        button.targetGraphic = image;

        image.rectTransform.sizeDelta = new Vector3(100, 30);
        (button.transform as RectTransform).sizeDelta = new Vector3(100, 30);

        text.rectTransform.sizeDelta = new Vector2(100, 30);
        text.color = Color.black;

        image.transform.SetParent(transform);
        text.transform.SetParent(transform);
        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(InputField))]
public class InputFieldWrapper : UIWrapperBase {

    public InputField inputField;
    public Image image;
    public Text text;

    public override void OnSpawn() {

        if(mainScript == null)
            mainScript = GetComponent<InputField>();

        inputField = mainScript as InputField;

        inputField.textComponent = null;
        inputField.text = "";
        inputField.onEndEdit.RemoveAllListeners();

        additionalScripts = new SpawnerOutput[] {
               LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
               LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper))
            };

        image = additionalScripts[0].script as Image;
        text = (additionalScripts[1].script as TextWrapper).mainScript as Text;

        inputField.gameObject.SetActive(true);

        inputField.textComponent = text;
        inputField.targetGraphic = image;
        (inputField.transform as RectTransform).sizeDelta = new Vector2(100, 30);

        text.color = Color.black;
        text.supportRichText = false;

        text.rectTransform.sizeDelta = new Vector2(100, 30);
        image.rectTransform.sizeDelta = new Vector2(100, 30);

        image.transform.SetParent(transform);
        text.transform.SetParent(transform);
        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectWrapper : UIWrapperBase {

    public ScrollRect scrollRect;
    public Image mainImage;
    public Image contentImage;
    public ScrollbarWrapper scrollBar;
    public RectTransform content;

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<ScrollRect>();
        }

        scrollRect = mainScript as ScrollRect;

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollbarWrapper))
        };

        // Main image.
        mainImage = additionalScripts[0].script as Image;

        // Image used for content ect.
        contentImage = additionalScripts[1].script as Image;
        scrollBar = additionalScripts[2].script as ScrollbarWrapper;

        content = LoadedData.GetSingleton<UIDrawer>().CreateEmptyGameObject().transform as RectTransform;
        contentImage.gameObject.AddComponent<Mask>();

        content.pivot = new Vector3(0.5f, 1);
        content.SetParent(contentImage.transform);
        mainImage.transform.SetParent(scrollRect.transform);
        contentImage.transform.SetParent(scrollRect.transform);
        scrollBar.transform.SetParent(scrollRect.transform);

        (contentImage.transform as RectTransform).sizeDelta = new Vector2(100, 150);
        scrollBar.transform.localPosition = new Vector2(60, 0);
        //sBW.transform.localPosition = new Vector3(300,0);

        scrollRect.viewport = contentImage.rectTransform;
        scrollRect.content = content;
        scrollRect.verticalScrollbar = scrollBar.mainScript as Scrollbar;
        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarWrapper : UIWrapperBase {

    public Scrollbar scrollBar;
    public Image imageMain;
    public Image imageHandler;
    public RectTransform scrollArea;

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<Scrollbar>();
        }

        scrollBar = mainScript as Scrollbar;

        scrollBar.direction = Scrollbar.Direction.BottomToTop;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image))
        };

        imageMain = additionalScripts[0].script as Image;
        imageHandler = additionalScripts[1].script as Image;
        scrollArea = LoadedData.GetSingleton<UIDrawer>().CreateEmptyGameObject().transform as RectTransform;

        imageMain.transform.SetParent(scrollBar.transform);
        scrollArea.SetParent(scrollBar.transform);
        imageHandler.transform.SetParent(scrollArea);

        (scrollBar.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (scrollArea.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (imageMain.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (imageHandler.transform as RectTransform).sizeDelta = new Vector2(10, 10);

        scrollBar.targetGraphic = imageHandler;
        scrollBar.handleRect = imageHandler.rectTransform;

        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(Dropdown))]
public class DropdownWrapper : UIWrapperBase {

    public Dropdown dropdown;
    public Image imageMain;
    public TextWrapper text;
    public ScrollRectWrapper scrollRect;
    public Toggle toggle;
    public TextWrapper tempText;

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<Dropdown>();
        }

        dropdown = mainScript as Dropdown;

        dropdown.interactable = true;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollRectWrapper)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Toggle)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)),
        };

        imageMain = additionalScripts[0].script as Image;
        text = additionalScripts[1].script as TextWrapper;
        scrollRect = additionalScripts[2].script as ScrollRectWrapper;
        toggle = additionalScripts[3].script as Toggle;
        tempText = additionalScripts[4].script as TextWrapper;

        (toggle.transform as RectTransform).sizeDelta = new Vector2(100, 20);

        imageMain.transform.SetParent(dropdown.transform);
        text.transform.SetParent(dropdown.transform);
        scrollRect.transform.SetParent(dropdown.transform);
        toggle.transform.SetParent(scrollRect.content.transform);
        tempText.transform.SetParent(toggle.transform);

        RectTransform sRRT = scrollRect.transform as RectTransform;
        sRRT.transform.localPosition = new Vector2(0, -90);

        RectTransform iMRT = imageMain.transform as RectTransform;
        iMRT.sizeDelta = new Vector2(100, 30);

        RectTransform dDRT = dropdown.transform as RectTransform;
        dDRT.sizeDelta = new Vector2(100, 30);

        scrollRect.gameObject.SetActive(false);

        dropdown.targetGraphic = imageMain;
        dropdown.template = scrollRect.transform as RectTransform;
        dropdown.captionText = text.mainScript as Text;
        dropdown.itemText = tempText.mainScript as Text;
    }
}

[RequireComponent(typeof(Toggle))]
public class ToggleWrapper : UIWrapperBase {

    public Toggle toggle;
    public Image imageMain;
    public Image imageToggle;

    public override void OnSpawn() {
        if(mainScript == null)
            mainScript = GetComponent<Toggle>();

        toggle = mainScript as Toggle;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image))
        };

        imageMain = additionalScripts[0].script as Image;
        imageToggle = additionalScripts[1].script as Image;

        (mainScript.transform as RectTransform).sizeDelta = new Vector2(30, 30);

        imageMain.rectTransform.sizeDelta = new Vector2(30, 30);
        imageToggle.rectTransform.sizeDelta = new Vector2(20, 20);

        imageToggle.color = Color.black;

        toggle.targetGraphic = imageMain;
        toggle.graphic = imageToggle;

        //AllignWrapperElements();
    }
}
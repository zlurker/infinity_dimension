using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AdditionalScriptData {
    public string name;
    public MonoBehaviour script;

    public AdditionalScriptData(string n, MonoBehaviour s) {
        name = n;
        script = s;
    }
}

public class UIWrapperBase : MonoBehaviour, IOnSpawn {

    // Main script in this. 
    //public MonoBehaviour mainScript;
    // Additional support scripts.
    public AdditionalScriptData[] scriptsData;

    public virtual void OnSpawn() {

    }

    /*public void AllignWrapperElements() {
        if(additionalScripts != null)
            for(int i = 0; i < additionalScripts.Length; i++) {
                additionalScripts[i].script.transform.SetParent(transform);
                additionalScripts[i].script.transform.localPosition = Vector3.zero;
            }
    }*/

    public void PopulateScriptDirectory(Dictionary<string, int> targetDict) {
        for(int i = 0; i < scriptsData.Length; i++)
            targetDict.Add(scriptsData[i].name, i);
    }
}

[RequireComponent(typeof(Text))]
public class TextWrapper : UIWrapperBase {

    public Text text;

    public override void OnSpawn() {
        if(text == null)
            text = GetComponent<Text>();

        text.text = "DEFAULTWORDS";
        text.font = Resources.Load("jd-bold") as Font;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        (text.transform as RectTransform).sizeDelta = new Vector2(100, 20);

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Text",text)
        };
    }
}

[RequireComponent(typeof(Button))]
public class ButtonWrapper : UIWrapperBase {

    public Button button;
    public Image image;
    public Text text;

    public override void OnSpawn() {

        if(button == null)
            button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        image = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        text = (LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)).script as TextWrapper).scriptsData[0].script as Text;

        button.targetGraphic = image;

        image.rectTransform.sizeDelta = new Vector3(100, 30);
        (button.transform as RectTransform).sizeDelta = new Vector3(100, 30);

        text.rectTransform.sizeDelta = new Vector2(100, 30);
        text.color = Color.black;

        image.transform.SetParent(transform);
        text.transform.SetParent(transform);
        //AllignWrapperElements();

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Button",button),
            new AdditionalScriptData("Image",image),
            new AdditionalScriptData("Text",text)
        };
    }

    public void ChangeButtonSize(Vector2 size) {
        (button.transform as RectTransform).sizeDelta = size;
        (image.transform as RectTransform).sizeDelta = size;
        (text.transform as RectTransform).sizeDelta = size;
    }
}

[RequireComponent(typeof(InputField))]
public class InputFieldWrapper : UIWrapperBase {

    public InputField inputField;
    public Image image;
    public Text text;

    public override void OnSpawn() {

        if(inputField == null)
            inputField = GetComponent<InputField>();

        inputField.textComponent = null;
        inputField.text = "";
        inputField.onEndEdit.RemoveAllListeners();

        image = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        text = (LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)).script as TextWrapper).scriptsData[0].script as Text;

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

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("InputField",inputField),
            new AdditionalScriptData("Image",image),
            new AdditionalScriptData("Text",text)
        };
    }
}

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectWrapper : UIWrapperBase {

    public ScrollRect scrollRect;
    public Image mainImage;
    public Image contentImage;
    public ScrollbarWrapper scrollBar;
    public UIMule content;

    public override void OnSpawn() {
        if(scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;


        // Main image.
        mainImage = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;

        // Image used for content ect.
        contentImage = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        scrollBar = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollbarWrapper)).script as ScrollbarWrapper;

        content = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(UIMule)).script as UIMule;
        contentImage.gameObject.AddComponent<Mask>();

        content.GetRectTransform().pivot = new Vector3(0.5f, 1);
        content.GetRectTransform().SetParent(contentImage.transform);
        mainImage.transform.SetParent(scrollRect.transform);
        contentImage.transform.SetParent(scrollRect.transform);
        scrollBar.transform.SetParent(scrollRect.transform);

        (contentImage.transform as RectTransform).sizeDelta = new Vector2(100, 150);
        scrollBar.transform.localPosition = new Vector2(60, 0);
        //sBW.transform.localPosition = new Vector3(300,0);

        scrollRect.viewport = contentImage.rectTransform;
        scrollRect.content = content.GetRectTransform();
        scrollRect.verticalScrollbar = scrollBar.scriptsData[0].script as Scrollbar;
        //AllignWrapperElements();

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("ScrollRect",scrollRect),
            new AdditionalScriptData("MainImage",mainImage),
            new AdditionalScriptData("ContentImage",contentImage),
            new AdditionalScriptData("Scrollbar",scrollBar),
            new AdditionalScriptData("Content",content)
        };
    }

    public void ChangeScrollRectSize(Vector2 dimensions) {
        scrollBar.ChangeScrollbarSize(new Vector2(10, dimensions.y));

        (scrollRect.transform as RectTransform).sizeDelta = dimensions;
        (mainImage.transform as RectTransform).sizeDelta = dimensions;
        (contentImage.transform as RectTransform).sizeDelta = dimensions;
    }
}

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarWrapper : UIWrapperBase {

    public Scrollbar scrollBar;
    public Image imageMain;
    public Image imageHandler;
    public UIMule scrollArea;

    public override void OnSpawn() {
        if(scrollBar == null)
            scrollBar = GetComponent<Scrollbar>();

        scrollBar.direction = Scrollbar.Direction.BottomToTop;

        imageMain = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        imageHandler = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        scrollArea = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(UIMule)).script as UIMule;

        imageMain.transform.SetParent(scrollBar.transform);
        scrollArea.GetRectTransform().SetParent(scrollBar.transform);
        imageHandler.transform.SetParent(scrollArea.GetRectTransform());

        (scrollBar.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (scrollArea.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (imageMain.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (imageHandler.transform as RectTransform).sizeDelta = new Vector2(10, 10);

        scrollBar.targetGraphic = imageHandler;
        scrollBar.handleRect = imageHandler.rectTransform;

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Scrollbar",scrollBar),
            new AdditionalScriptData("MainImage",imageMain),
            new AdditionalScriptData("HandlerImage",imageHandler),
            new AdditionalScriptData("ScrollArea",scrollArea)
        };
    }

    public void ChangeScrollbarSize(Vector2 dimensions) {
        (scrollBar.transform as RectTransform).sizeDelta = dimensions;
        (imageMain.transform as RectTransform).sizeDelta = dimensions;
        (scrollArea.transform as RectTransform).sizeDelta = dimensions;
    }
}

[RequireComponent(typeof(Dropdown))]
public class DropdownWrapper : UIWrapperBase {

    public Dropdown dropdown;
    public Image imageMain;
    public Text text;
    public ScrollRectWrapper scrollRect;
    public Toggle toggle;
    public Text tempText;

    public override void OnSpawn() {
        if(dropdown == null)
            dropdown = GetComponent<Dropdown>();

        dropdown.interactable = true;

        imageMain = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        text = (LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)).script as TextWrapper).text;
        scrollRect = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollRectWrapper)).script as ScrollRectWrapper;
        toggle = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Toggle)).script as Toggle;
        tempText = (LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)).script as TextWrapper).text;

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
        dropdown.captionText = text;
        dropdown.itemText = tempText;

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Dropdown",dropdown),
            new AdditionalScriptData("MainImage",imageMain),
            new AdditionalScriptData("MainText",text),
            new AdditionalScriptData("ScrollRect",scrollRect),
            new AdditionalScriptData("Toggle",toggle),
            new AdditionalScriptData("TempText",tempText),
        };
    }
}

[RequireComponent(typeof(Toggle))]
public class ToggleWrapper : UIWrapperBase {

    public Toggle toggle;
    public Image imageMain;
    public Image imageToggle;

    public override void OnSpawn() {
        if(toggle == null)
            toggle = GetComponent<Toggle>();

        imageMain = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;
        imageToggle = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)).script as Image;

        (toggle.transform as RectTransform).sizeDelta = new Vector2(30, 30);

        imageMain.rectTransform.sizeDelta = new Vector2(30, 30);
        imageToggle.rectTransform.sizeDelta = new Vector2(20, 20);

        imageToggle.color = Color.black;

        toggle.targetGraphic = imageMain;
        toggle.graphic = imageToggle;

        imageMain.transform.SetParent(toggle.transform);
        imageToggle.transform.SetParent(toggle.transform);

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Toggle",toggle),
            new AdditionalScriptData("ImageMain",imageMain),
            new AdditionalScriptData("ImageToggle",imageToggle)
        };
        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(Image))]
public class WindowsWrapper : UIWrapperBase, IPointerDownHandler, IDragHandler {

    public Image windowsGraphic;
    public ButtonWrapper deleteButton;
    public UIMule content;
    public Text windowsText;

    Vector2 pointInObject;
    Vector3 trackedPos;

    Vector2 windowsSize;
    // X - Top, Y - Bottom, Z - Left, W - Right
    Vector4 contentOffset;

    public override void OnSpawn() {
        if(windowsGraphic == null)
            windowsGraphic = GetComponent<Image>();

        deleteButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper)).script as ButtonWrapper;
        content = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(UIMule)).script as UIMule;
        windowsText = (LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)).script as TextWrapper).text;

        deleteButton.ChangeButtonSize(new Vector2(15, 15));
        deleteButton.image.color = Color.red;
        deleteButton.text.text = "";
        windowsGraphic.color = new Color(1, 1, 1, 0.5f);

        ChangeWindowsHeaderSize(new Vector4(25,6,6,6));
        ChangeWindowsContentSize(new Vector2(100, 100));

        deleteButton.transform.SetParent(transform);
        content.transform.SetParent(transform);
        windowsText.transform.SetParent(transform);
        content.GetRectTransform().sizeDelta = new Vector2();

        deleteButton.button.onClick.RemoveAllListeners();
        deleteButton.button.onClick.AddListener(() => { gameObject.SetActive(false); });

        scriptsData = new AdditionalScriptData[] {
            new AdditionalScriptData("Windows",windowsGraphic),
            new AdditionalScriptData("DeleteButton",deleteButton),
            new AdditionalScriptData("Content",content),
            new AdditionalScriptData("WindowsText",windowsText)
        };

        transform.SetAsLastSibling();
    }

    public void ChangeWindowsHeaderSize(Vector4 offset) {
        contentOffset = offset;
    }

    public void ChangeWindowsContentSize(Vector2 size) {
        (windowsGraphic.transform as RectTransform).sizeDelta = new Vector2(size.x + contentOffset.z + contentOffset.w, size.y + contentOffset.x + contentOffset.y);
       
        deleteButton.transform.position = UIDrawer.UINormalisedPosition(windowsGraphic.transform as RectTransform, new Vector2(0.95f, 1f)) - new Vector3(0, contentOffset.x / 2);
        windowsText.transform.position = UIDrawer.UINormalisedPosition(windowsGraphic.transform as RectTransform, new Vector2(0.5f, 1f)) - new Vector3(0, contentOffset.x / 2);
        (windowsText.transform as RectTransform).sizeDelta = new Vector2(size.x - contentOffset.z - contentOffset.w, contentOffset.x * 0.9f);
        content.transform.position = UIDrawer.UINormalisedPosition(windowsGraphic.transform as RectTransform, new Vector2(0f, 1f)) - new Vector3(-contentOffset.z, contentOffset.x);
        
        windowsSize = size;
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        transform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out pointInObject);
    }

    public virtual void OnDrag(PointerEventData eventData) {
        Vector2 currMousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out currMousePos);
        transform.localPosition = currMousePos - pointInObject;
    }
}
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
        if(mainScript == null)
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

        if(mainScript == null)
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

        if(mainScript == null)
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

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectWrapper : UIWrapperBase {

    public RectTransform content;

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<ScrollRect>();
        }

        ScrollRect sR = mainScript as ScrollRect;

        sR.horizontal = false;
        sR.vertical = true;
        sR.movementType = ScrollRect.MovementType.Elastic;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollbarWrapper))
        };

        // Main image.
        Image iM = additionalScripts[0].script as Image;

        // Image used for content ect.
        Image iVP = additionalScripts[1].script as Image;
        ScrollbarWrapper sB  = additionalScripts[2].script as ScrollbarWrapper;

        content = LoadedData.GetSingleton<UIDrawer>().CreateEmptyGameObject().transform as RectTransform;
        iVP.gameObject.AddComponent<Mask>();
       
        content.SetParent(iVP.transform);
        iM.transform.SetParent(sR.transform);
        iVP.transform.SetParent(sR.transform);
        sB.transform.SetParent(sR.transform);

        (iVP.transform as RectTransform).sizeDelta = new Vector2(100, 150);
        sB.transform.localPosition = new Vector2(60, 0);
        //sBW.transform.localPosition = new Vector3(300,0);

        sR.viewport = iVP.rectTransform;
        sR.content = content;
        sR.verticalScrollbar = sB.mainScript as Scrollbar;
        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarWrapper : UIWrapperBase {

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<Scrollbar>();
        }

        Scrollbar sR = mainScript as Scrollbar;

        sR.direction = Scrollbar.Direction.BottomToTop;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image))
        };

        Image iM = additionalScripts[0].script as Image;
        Image iH = additionalScripts[1].script as Image;
        RectTransform sArea = LoadedData.GetSingleton<UIDrawer>().CreateEmptyGameObject().transform as RectTransform;

        iM.transform.SetParent(sR.transform);
        sArea.SetParent(sR.transform);
        iH.transform.SetParent(sArea);

        (sR.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (sArea.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (iM.transform as RectTransform).sizeDelta = new Vector2(10, 150);
        (iH.transform as RectTransform).sizeDelta = new Vector2(10, 10);

        sR.targetGraphic = iH;
        sR.handleRect = iH.rectTransform;

        //AllignWrapperElements();
    }
}

[RequireComponent(typeof(Dropdown))]
public class DropdownWrapper : UIWrapperBase {

    public override void OnSpawn() {
        if(mainScript == null) {
            mainScript = GetComponent<Dropdown>();            
        }

        Dropdown dD = mainScript as Dropdown;

        dD.interactable = true;

        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollRectWrapper)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Toggle)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper)),
        };

        Image iM = additionalScripts[0].script as Image;
        TextWrapper t = additionalScripts[1].script as TextWrapper;
        ScrollRectWrapper sR = additionalScripts[2].script as ScrollRectWrapper;
        Toggle tog = additionalScripts[3].script as Toggle;
        TextWrapper tempT = additionalScripts[4].script as TextWrapper;

        (tog.transform as RectTransform).sizeDelta = new Vector2(100, 20);
        
        iM.transform.SetParent(dD.transform);
        t.transform.SetParent(dD.transform);
        sR.transform.SetParent(dD.transform);
        tog.transform.SetParent(sR.content.transform);
        tempT.transform.SetParent(tog.transform);

        RectTransform sRRT = sR.transform as RectTransform;
        sRRT.transform.localPosition = new Vector2(0, -90);

        RectTransform iMRT = iM.transform as RectTransform;
        iMRT.sizeDelta = new Vector2(100, 30);

        RectTransform dDRT = dD.transform as RectTransform;
        dDRT.sizeDelta = new Vector2(100, 30);

        sR.gameObject.SetActive(false);

        dD.targetGraphic = iM;
        dD.template = sR.transform as RectTransform;
        dD.captionText = t.mainScript as Text;
        dD.itemText = tempT.mainScript as Text;
    }
}

[RequireComponent(typeof(Toggle))]
public class ToggleWrapper : UIWrapperBase {

    public override void OnSpawn() {
        if(mainScript == null) 
            mainScript = GetComponent<Toggle>();
        
        additionalScripts = new SpawnerOutput[] {
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image)),
            LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image))
        };

        Image iM = additionalScripts[0].script as Image;
        Image iT = additionalScripts[1].script as Image;

        iM.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
        iT.rectTransform.sizeDelta = new Vector2(0.4f, 0.4f);

        iT.color = Color.black;

        (mainScript as Toggle).targetGraphic = iM;
        (mainScript as Toggle).graphic = iT;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class Layer {
    public List<string> uE; //uiElements

    public Layer() {
        uE = new List<string>();
    }
}

public class UIDrawer : Spawner, ISingleton {
    public static Canvas t; //target
    public DH uiCreator;
    public Layer uL; //uiLayer

    public override void CreateDefaultSettings() {
        oDS = new ObjectDefaultSettings[] {
            new ObjectDefaultSettings<Text>((t,sOC) =>{
                t.text = "DEFAULTWORDS";
                t.font = Resources.Load("jd-bold") as Font;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.horizontalOverflow = HorizontalWrapMode.Wrap;
                t.alignment = TextAnchor.MiddleCenter;
                t.color = Color.black;
                (t.transform as RectTransform).sizeDelta = new Vector2(100,20);
                //Debug.Log(t.text);
            }),

            new ObjectDefaultSettings<InputField>((t,sOC) =>{
                t.textComponent = null;
                t.text = "";
                t.onEndEdit.RemoveAllListeners();

                sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>());
                sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>());

                t.gameObject.SetActive(true);

                t.textComponent = GetCType<Text>(sOC);
                t.targetGraphic = GetCType<Image>(sOC);
                (t.transform as RectTransform).sizeDelta = new Vector2(100,30);

                GetCType<Text>(sOC).color = Color.black;
                GetCType<Text>(sOC).supportRichText = false;
                GetCType<Text>(sOC).transform.SetParent(t.transform);
                GetCType<Text>(sOC).rectTransform.sizeDelta = new Vector2(100, 30);

                GetCType<Image>(sOC).rectTransform.sizeDelta = new Vector2(100, 30);
            }),

            new ObjectDefaultSettings<Button>((t, sOC)=>{
                t.onClick.RemoveAllListeners();
                sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>());
                sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>());
                t.targetGraphic = GetCType<Image>(sOC);

                GetCType<Image>(sOC).rectTransform.sizeDelta = new Vector3(100, 30);
                (t.transform as RectTransform).sizeDelta = new Vector3(100, 30);

                GetCType<Text>(sOC).transform.SetParent(t.transform);
                GetCType<Text>(sOC).rectTransform.sizeDelta = new Vector2(100, 30);
                GetCType<Text>(sOC).color = Color.black;
            }),

            new ObjectDefaultSettings<WindowsScript>((t, sOC)=>{
                 //t.transform.name = "Panel";
                 sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>());
                 t.transform.SetAsLastSibling();

                GetCType<Image>(sOC).rectTransform.sizeDelta = new Vector2(100,40);
                (t.transform as RectTransform).sizeDelta = new Vector2(100,40);
                 //sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>());
            }),

            new ObjectDefaultSettings<LinearLayout>((t, sOC)=>{
                 //t.transform.name = "Panel";
                 (t.transform as RectTransform).sizeDelta = new Vector2(0,0);
                 //sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>());
            }),

            new ObjectDefaultSettings<Line>((t, sOC)=>{
                 //t.transform.name = "Panel";
                 (t.transform as RectTransform).sizeDelta = new Vector2(0,0);
                 sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>());
                 //sOC.AddMultiple(Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>());
            })
        };
    }

    public override ScriptableObject CreateScriptedObject(Type[] type) {
        ScriptableObject instance = base.CreateScriptedObject(type);

        UpdateMainObject(instance);

        return instance;
    }

    public override ScriptableObject CreateScriptedObject(MonoBehaviour[][] scripts) {
        ScriptableObject instance = base.CreateScriptedObject(scripts);
        UpdateMainObject(instance);

        return instance;
    }
    public static void ChangeUISize(ScriptableObject target, Type type, Vector2 size) {
        (GetCType(target, type).transform as RectTransform).sizeDelta = size;
        UpdateMainObject(target);
    }

    public static void ChangeUISize(ScriptableObject target, Vector2 size) {
        for(int i = 0; i < target.scripts.Length; i++)
            (target.scripts[i].transform as RectTransform).sizeDelta = size;

        UpdateMainObject(target);
    }

    public static void UpdateMainObject(ScriptableObject target) {
        Vector2 dimensions = new Vector2();

        for(int i = 0; i < target.scripts.Length; i++) {
            RectTransform rT = target.scripts[i].transform as RectTransform;

            for(int j = 0; j < 2; j++)
                if(rT.sizeDelta[j] > dimensions[j])
                    dimensions[j] = rT.sizeDelta[j];
        }

        (target.transform as RectTransform).sizeDelta = dimensions;
    }

    public void ModifyLayer(int layerNumber, string groupName = "", bool removeAllSucessors = true) {
        while(layerNumber >= uL.uE.Count)
            uL.uE.Add("");

        int loopCount = removeAllSucessors ? uL.uE.Count : layerNumber + 1;

        for(int i = layerNumber; i < loopCount; i++)
            if(uL.uE[i] != "") {
                /*PatternControl.i.Pattern_Args(null, new object[][] {
                new object[]{ Patterns.GROUP_PATTERN, uL.uE[i], GroupArgs.REMOVE_ALL_CURRENT_OBJECTS }
                });
                uL.uE[i] = "";*/
            }

        uL.uE[layerNumber] = groupName;
    }

    public static Vector3 UINormalisedPosition(Vector3 c) {//coordinates: Returns back position to the decimal of 1.
        return UINormalisedPosition(t.transform as RectTransform, c);
    }

    public static Vector3 UINormalisedPosition(RectTransform target, Vector2 c) {//coordinates: Returns back position to the decimal of 1.
        c -= target.pivot;
        for(int i = 0; i < 2; i++) {
            c[i] = (c[i] / 1) * target.sizeDelta[i];
            c[i] += target.position[i];
        }
        return c;
    }

    public new void RunOnStart() {
        t = FindObjectOfType<Canvas>();
    }

    public new void RunOnCreated() {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };
        uL = new Layer();
    }

    public override ScriptableObject CustomiseBaseObject() {
        ScriptableObject baseObject = base.CustomiseBaseObject();
        baseObject.transform.SetParent(t.transform);
        return baseObject;
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","What the fuck nigga")
        };
    }

    public void SetValues(RuntimeParameters[] values) {
        Debug.Log("UICalled");
    }
}
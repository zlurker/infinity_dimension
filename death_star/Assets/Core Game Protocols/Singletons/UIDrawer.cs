using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class UIDrawer : Spawner, ISingleton {
    public static Canvas t; //target
    public static Dictionary<Type, int> butInpIds;

    public SpawnerOutput CreateUIObject(Type type) {

        SpawnerOutput inst = CreateScriptedObject(type);

        // Additional actions required by the individual UI elements.
        if(type == typeof(Text))
            TextBoxHandler(inst.script as Text);

        if(type == typeof(InputField))
            inst.additionalScripts = InputFieldHandler(inst.script as InputField);

        if(type == typeof(Button))
            inst.additionalScripts = ButtonHandler(inst.script as Button);

        if(type == typeof(WindowsScript))
            inst.additionalScripts = WindowScriptHandler(inst.script as WindowsScript);

        if(type == typeof(LinearLayout))
            LinearLayoutHandler(inst.script as LinearLayout);

        inst.script.transform.SetParent(t.transform);

        if(inst.additionalScripts != null)
            for(int i = 0; i < inst.additionalScripts.Length; i++) {
                inst.additionalScripts[i].script.transform.SetParent(inst.script.transform);
                inst.additionalScripts[i].script.transform.localPosition = Vector3.zero;
            }

        return inst;
    }

    public void TextBoxHandler(Text t) {
        t.text = "DEFAULTWORDS";
        t.font = Resources.Load("jd-bold") as Font;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.black;
        (t.transform as RectTransform).sizeDelta = new Vector2(100, 20);
    }

    public SpawnerOutput[] InputFieldHandler(InputField iF) {
        iF.textComponent = null;
        iF.text = "";
        iF.onEndEdit.RemoveAllListeners();

        SpawnerOutput[] output = new SpawnerOutput[] {
             CreateUIObject(typeof(Image)),
             CreateUIObject(typeof(Text))
        };

        Image i = output[0].script as Image;
        Text t = output[1].script as Text;

        iF.gameObject.SetActive(true);

        iF.textComponent = t;
        iF.targetGraphic = i;
        (iF.transform as RectTransform).sizeDelta = new Vector2(100, 30);

        t.color = Color.black;
        t.supportRichText = false;
        t.transform.SetParent(t.transform);
        t.rectTransform.sizeDelta = new Vector2(100, 30);

        i.rectTransform.sizeDelta = new Vector2(100, 30);

        return output;
    }

    public SpawnerOutput[] ButtonHandler(Button b) {
        b.onClick.RemoveAllListeners();

        SpawnerOutput[] output = new SpawnerOutput[] {
             CreateUIObject(typeof(Image)),
             CreateUIObject(typeof(Text))
        };

        Image i = output[0].script as Image;
        Text t = output[1].script as Text;

        b.targetGraphic = i;

        i.rectTransform.sizeDelta = new Vector3(100, 30);
        (b.transform as RectTransform).sizeDelta = new Vector3(100, 30);

        t.transform.SetParent(t.transform);
        t.rectTransform.sizeDelta = new Vector2(100, 30);
        t.color = Color.black;

        return output;
    }

    public SpawnerOutput[] WindowScriptHandler(WindowsScript wS) {

        SpawnerOutput[] output = new SpawnerOutput[] {
             CreateUIObject(typeof(Image))
        };

        Image i = output[0].script as Image;
        wS.transform.SetAsLastSibling();

        i.rectTransform.sizeDelta = new Vector2(100, 40);
        (wS.transform as RectTransform).sizeDelta = new Vector2(100, 40);

        return output;
    }

    public void LinearLayoutHandler(LinearLayout lL) {
        (lL.transform as RectTransform).sizeDelta = new Vector2(0, 0);
    }

    // A wrapped to assist us in getting elements nested within.
    public static T GetTypeInElement<T>(SpawnerOutput target) {

        if(target.script is T)
            return (T)(object)target.script;

        if(target.script is Button || target.script is InputField) 
            return (T)(object)target.additionalScripts[butInpIds[typeof(T)]].script;
        

        return (T)(object)null;
    }

    /*public override ScriptableObject CreateScriptedObject(Type[] type) {
        ScriptableObject instance = base.CreateScriptedObject(type);

        UpdateMainObject(instance);

        return instance;
    }

    public override ScriptableObject CreateScriptedObject(MonoBehaviour[][] scripts) {
        ScriptableObject instance = base.CreateScriptedObject(scripts);
        UpdateMainObject(instance);

        return instance;
    }*/

    /*public static void ChangeUISize(ScriptableObject target, Type type, Vector2 size) {
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
    }*/

    public static void ChangeUISize(SpawnerOutput target, Vector2 size) {
        (target.script.transform as RectTransform).sizeDelta = size;

        for(int i = 0; i < target.additionalScripts.Length; i++)
            (target.additionalScripts[i].script.transform as RectTransform).sizeDelta = size;
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

        butInpIds = new Dictionary<Type, int>();

        butInpIds.Add(typeof(Image), 0);
        butInpIds.Add(typeof(Text), 1);
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","What the fuck nigga")
        };
    }
}
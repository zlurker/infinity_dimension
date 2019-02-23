using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class Layer
{
    public List<string> uE; //uiElements

    public Layer()
    {
        uE = new List<string>();
    }
}

public class UIDrawer : Spawner, ISingleton, IPlayerEditable
{
    public static Canvas t; //target
    public DH uiCreator;
    public Layer uL; //uiLayer

    public override void CreateDefaultSettings()
    {
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
            })
        };
    }

    public override void CreateOnSpawnDelegates()
    {
        oSD = new OnSpawnDelegates[]
        {
            new OnSpawnDelegates("Position",new DH((p) =>
            {
                ScriptableObject p0 = p[0] as ScriptableObject;
                Vector3 p1 = (Vector3) p[1];

                p0.transform.position = p1;
            })),
            new OnSpawnDelegates("UIPosition",new DH((p) =>
            {
                ScriptableObject p0 = p[0] as ScriptableObject;
                Vector3 p1 = (Vector3) p[1];

                p0.transform.position = UIDrawer.UINormalisedPosition(p1);
            }))
        };
    }

    public override ScriptableObject CreateScriptedObject(MonoBehaviour[][] scripts, DelegateInfo[] onSpawn = null)
    {
        Vector2 dimensions = new Vector2();
        ScriptableObject instance = base.CreateScriptedObject(scripts, onSpawn);

        for (int i =0; i < instance.scripts.Length; i++)
        {
            RectTransform rT = instance.scripts[i].transform as RectTransform;

            for (int j = 0; j < 2; j++)
                if (rT.sizeDelta[j] > dimensions[j])
                    dimensions[j] = rT.sizeDelta[j];
        }

        (instance.transform as RectTransform).sizeDelta = dimensions;
        return instance;
    }

    public void ModifyLayer(int layerNumber, string groupName = "", bool removeAllSucessors = true)
    {
        while (layerNumber >= uL.uE.Count)
            uL.uE.Add("");

        int loopCount = removeAllSucessors ? uL.uE.Count : layerNumber + 1;

        for (int i = layerNumber; i < loopCount; i++)
            if (uL.uE[i] != "")
            {
                /*PatternControl.i.Pattern_Args(null, new object[][] {
                new object[]{ Patterns.GROUP_PATTERN, uL.uE[i], GroupArgs.REMOVE_ALL_CURRENT_OBJECTS }
                });
                uL.uE[i] = "";*/
            }

        uL.uE[layerNumber] = groupName;
    }

    public static Vector3 UINormalisedPosition(Vector3 c)
    {//coordinates: Returns back position to the decimal of 1.

        for (int i = 0; i < 2; i++)
        {
            c[i] = (c[i] / 1) * t.pixelRect.size[i];
            c[i] += t.pixelRect.min[i];
        }

        return c;
    }

    public MethodInfo GetMainMethod()
    {
        return GetType().GetMethod("ModifyLayer");
    }

    public void Invoke(object[] p)
    {

    }


    public new void RunOnStart()
    {
        t = FindObjectOfType<Canvas>();
    }

    public new void RunOnCreated()
    {
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };
        uL = new Layer();
    }

    public override ScriptableObject CustomiseBaseObject()
    {
        ScriptableObject baseObject = base.CustomiseBaseObject();
        baseObject.transform.SetParent(t.transform);
        return baseObject;
    }

    public RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","What the fuck nigga"),

        };
    }

    public RuntimeParameters[] GetRawRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("UI","Seriously nigga"),

        };
    }

    public void Invoke(Iterator[] parameters) {
        throw new NotImplementedException();
    }
}
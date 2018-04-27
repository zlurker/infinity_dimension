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

    public static UIDrawer i; //instance
    public static Canvas t; //target
    public DH uiCreator;
    public Layer uL; //uiLayer

    public void ModifyLayer(int layerNumber, string groupName = "",bool removeAllSucessors = true)
    {
        while (layerNumber >= uL.uE.Count)
            uL.uE.Add("");

        int loopCount = removeAllSucessors ? uL.uE.Count : layerNumber + 1;

        for (int i = layerNumber; i < loopCount; i++)
            if (uL.uE[i] != "")
            {
                PatternControl.i.Pattern_Args(null, new object[][] {
                new object[]{ Patterns.GROUP_PATTERN, uL.uE[i], GroupArgs.REMOVE_ALL_CURRENT_OBJECTS }
                });
                uL.uE[i] = "";
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

    public void UIInterfaceTest(int type)
    {

    }

    public MethodInfo GetMainMethod()
    {
        return GetType().GetMethod("UIInterfaceTest");
    }

    public void Invoke(object[] p)
    {

    }

    public object ReturnInstance()
    {
        return i;
    }

    public void RunOnStart()
    {
        t = FindObjectOfType<Canvas>();
    }

    public void RunOnCreated()
    {
        i = this;
        DontDestroyOnLoad(gameObject);
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };
        uL = new Layer();
    }

    public override ScriptableObject CustomiseBaseObject()
    {
        ScriptableObject baseObject = base.CustomiseBaseObject();
        baseObject.transform.SetParent(t.transform);
        return baseObject;
    }
}
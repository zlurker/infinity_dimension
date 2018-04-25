using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

/*public class SetOnSpawnParameters {
    public Type t; //type
    public object v; //value

    public SetOnSpawnParameters(Type type, object value) {
        t = type;
        v = value;
    }
}*/

/*public class TextSettings : ObjectSettings
{
    public override Modifier[] SetDefaultValues()
    {
        return new Modifier[] {
            new Modifier(0,"Default Text"),
            new Modifier(1,Resources.Load("jd-bold") as Font)
        };
    }

    public override void SetObjectValues(MonoBehaviour t, Modifier[] mods)
    {
        Text s = t as Text;
        if (mods != null)
            for (int i = 0; i < mods.Length; i++)
                switch (mods[i].v)
                {
                    case 0:
                        s.text = mods[i].vV as string;
                        break;
                    case 1:
                        s.font = mods[i].vV as Font;
                        break;
                }
    }

}*/

/*public class TextPointer: PointerHolderCreator
{
    //public override void CreatePointerList(MonoBehaviour target, PointerHolder[] setArray)
    {
        Text t = target as Text;
        setArray[0].p = new Action(() => GenericModifier<string>.ModifyValue(ref t.text, setArray[0].tP));
    }
}*/

/*public class TextPointer : PointerHolderCreatorBase
{
    public TextPointer()
    {
        p = new PointerHolder[] {
            new PointerHolder<Text,string>((t,v) => { t.text = v; },"defaultvalue"),
            new PointerHolder<Text,int>((t,v) => { t.fontSize = v; }),
            new PointerHolder<Text,Font>((t,v) => { t.font = v; },Resources.Load("jd-bold"))
        };
    }
}*/

/*public class ButtonPointer: PointerHolderCreatorBase
{
    public ButtonPointer()
    {
        p = new PointerHolder[]
        {
            new PointerHolder<Button,DH>((b,v) => { b.onClick.AddListener(v.Invoke); })
        };
    }
}*/

public class UIDrawer : Spawner, ISingleton, IPlayerEditable
{

    public static UIDrawer i; //instance
    public static Canvas t; //target
    public DH uiCreator;

    /* public override void CreateTypePool()
     {

         bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

         tP = new TypePool[] {
             new TypePool(new TypeIterator[] { new TypeIterator<Image>() }, "Image"),
             new TypePool(new TypeIterator[] { new TypeIterator<Image>(), new TypeIterator<Button>()},"Button"),

             new TypePool(new TypeIterator[] {
                 new TypeIterator<Text>((t) => {
                     t.text = "DEFAULTWORDS";
                     t.font = Resources.Load("jd-bold") as Font;
                     t.verticalOverflow = VerticalWrapMode.Overflow;
                     t.horizontalOverflow = HorizontalWrapMode.Wrap;                 
                 }) }, "Text"),

             new TypePool(new TypeIterator[]{ new TypeIterator<Image>(),
                 new TypeIterator<InputField>((t) => {
                     Text tC = GetCType<Text>(Spawn("Text"));
                     t.textComponent = tC;
                     tC.color = Color.black;
                     tC.supportRichText = false;
                     tC.transform.SetParent(t.transform);
                     tC.transform.position = Vector3.zero;
                 })},"InputField")
         };
     }*/

    /*public override PoolElement Spawn(string p)
    {
        PoolElement inst = base.Spawn(p);
        SetVariable(inst,uiCreator,new object[] { t });

        return inst;
        //return Spawn(p, new Vector3(), t.transform);
    }*/

    /*public override PoolElement Spawn(string p, Vector3 l, float d)
    { //pool, location, duration
        PoolElement iR = Spawn(p, l, t.transform);
        TimeHandler.i.AddNewTimerEvent(new TimeData(Time.time + d, new DH(Remove, new object[] { iR })));

        return iR;
    }*/

    /*public PoolElement Spawn(string p, bool uNP, Vector3 l)
    { //pool, location, useNormalisedPosition
        if (uNP)
            l = UINormalisedPosition(l);

        //return Spawn(p, l, t.transform);
        return Spawn(p, l, t.transform);
    }*/

    /*public PoolElement Spawn(string p, bool uNP, Vector3 l, ObjectSettings[] sP) { //pool, location, useNormalisedosition, spawnParameters
        if (uNP)
            l = UINormalisedPosition(l);

        PoolElement iR = Spawn(p, l, t.transform);

        for (int i = 0; i < sP.Length; i++)
            SetUIComponent(iR.o, sP[i].t, sP[i].v);

        return iR;
    }*/

    public static Vector3 UINormalisedPosition(Vector3 c)
    {//coordinates: Returns back position to the decimal of 1.

        for (int i = 0; i < 2; i++)
        {
            c[i] = (c[i] / 1) * t.pixelRect.size[i];
            c[i] += t.pixelRect.min[i];
        }

        return c;
    }

    /*public void SetUIComponent(MonoBehaviour o, object p)
    {

        if (o is Text)
            (o as Text).text = p as string;


        if (o is Image)
            (o as Image).sprite = p as Sprite;

        //For button, parameter is a new DH.
        if (o is Button)
            (o as Button).onClick.AddListener((p as DH).Invoke);
    }*/

    /*public void SetUIComponent(MonoBehaviour[] o, Type t, object p)
    {
        for (int i = 0; i < o.Length; i++)
            if (o[i].GetType() == t)
                SetUIComponent(o[i], p);
    }*/

    /*public object GetUIComponent(MonoBehaviour obj)
    {

        if (obj is Text)
            return (obj as Text).text;

        if (obj is Image)
            return (obj as Image).sprite;

        if (obj is InputField)
            return (obj as InputField).text;

        return null;
    }*/

    /*public void ReplaceUIGroup(string name, PoolElement[] replaceWith)
    {
        PatternControl.i.Pattern_Args(replaceWith,
            new object[][] {
                new object[] { "GROUPPATTERN", name, "REMOVE_ALL_CURRENT_OBJECTS,ADD_PARAMETER_OBJECTS"}
            }
            );
    }*/

    public void Fire(object[] parameters)
    {

    }

    public MethodInfo GetMainMethod()
    {
        Debug.Log("TEST WORKS?");
        return null;
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
        //Debug.Log("Called");
    }

    public void RunOnCreated()
    {
        i = this;
        DontDestroyOnLoad(gameObject);
        bB = new Type[] { typeof(RectTransform), typeof(CanvasRenderer) };

        /*uiCreator = new DH((o) =>
        {
            //PoolElement arg0 = (PoolElement)o[0];
            Transform arg1 = ((Canvas)o[1]).transform;

            //arg0.o[0].s.transform.SetParent(arg1);
        });*/
        
    }

    //public override object CreateBaseObject(object p)
    //{
    //  GameObject inst = new GameObject("ScriptBaseHolder", new Type[] { typeof(RectTransform), typeof(CanvasRenderer) });
    //  inst.transform.SetParent(t.transform);
    //  return inst;
    //}

    public override ScriptableObject CustomiseBaseObject()
    {
        //Debug.Log("Called before Run");
        ScriptableObject baseObject = base.CustomiseBaseObject();
        baseObject.transform.SetParent(t.transform);
        return baseObject;
    }
}
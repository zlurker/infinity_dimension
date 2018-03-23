using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUICommands : MonoBehaviour
{

    public Font font;
    public delegate void test();
    Text instance;
    void Start()
    {
        PatternControl.i.Pattern_Args(new PoolElement[] {
            UIDrawer.i.Spawn("Image",true, new Vector2(0.5f, 0.9f)),
            //UIDrawer.i.Spawn("Text",true, new Vector2(0.5f, 0.8f),new SetOnSpawnParameters[] { new SetOnSpawnParameters(typeof(Text),"Suck my dick")})
        }, new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Introduction", GroupArgs.PARENT_PARAMETER_OBJECTS,GroupArgs.ADD_PARAMETER_OBJECTS }
        });




        instance = Iterator.ReturnObject<ScriptIterator>(UIDrawer.i.Spawn("Text", true, new Vector3(0.5f, 0.9f)).o, "Text").s as Text;
        PoolElement[] buttons = new PoolElement[LoadedData.gIPEI.Length];

        for (int i = 0; i < LoadedData.uL.Length; i++)
        {
            PoolElement inst = UIDrawer.i.Spawn("Button");
            string name = LoadedData.uL[i].GetType().Name;
            UIDrawer.i.SetPointer(inst, "Button", "listener", new DH((o)=> { instance.text = name; Debug.Log(i); }));
            buttons[i] = inst;
        }


        PatternControl.i.Pattern_Args(buttons,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Secondary", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.i.UINormalisedPosition(new Vector3(0.1f, 0.85f)), new Vector3(-1.5f,-5f) }
        });

        PoolElement test = UIDrawer.i.Spawn("Text");
        UIDrawer.i.SetPointer(test, "Text", "text", "Mother fucking shit");
        //UIDrawer.i.SetUIComponent(test,typeof(Image), null);
    }

    void Testing(object[] p)
    {
        Debug.Log("Able to invoke?!?!?!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

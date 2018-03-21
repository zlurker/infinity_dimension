using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUICommands : MonoBehaviour
{

    public Font font;
    public delegate void test();

    // Use this for initialization
    void Start()
    {
        PatternControl.i.Pattern_Args(new PoolElement[] {
            UIDrawer.i.Spawn("Image",true, new Vector2(0.5f, 0.9f)),
            //UIDrawer.i.Spawn("Text",true, new Vector2(0.5f, 0.8f),new SetOnSpawnParameters[] { new SetOnSpawnParameters(typeof(Text),"Suck my dick")})
        }, new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Introduction", GroupArgs.PARENT_PARAMETER_OBJECTS,GroupArgs.ADD_PARAMETER_OBJECTS }
        });


        PatternControl.i.Pattern_Args(new PoolElement[] {
            UIDrawer.i.Spawn("Text"),
            UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text")
        }, new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Secondary", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.i.UINormalisedPosition(new Vector3(0.1f, 0.85f)), new Vector3(-1.5f,-5f) }
        });

        UIDrawer.i.Spawn("Text");
        //UIDrawer.i.SetUIComponent(test,typeof(Image), null);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

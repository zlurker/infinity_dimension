using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUICommands : MonoBehaviour {

    // Use this for initialization
    void Start() {
        PatternControl.i.Pattern_Args(new PoolElement[] {
            UIDrawer.i.Spawn("Image", UIDrawer.i.ReturnPosition(new Vector2(0.5f, 0.9f))),
            UIDrawer.i.Spawn("Text", UIDrawer.i.ReturnPosition(new Vector2(0.5f, 0.8f)))
        }, new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Introduction", GroupArgs.PARENT_PARAMETER_OBJECTS,GroupArgs.ADD_PARAMETER_OBJECTS }
        });


        PatternControl.i.Pattern_Args(new PoolElement[] {
            UIDrawer.i.Spawn("Text",UIDrawer.i.ReturnPosition(new Vector3(0.1f, 0.85f))),
            UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text"), UIDrawer.i.Spawn("Text")
        }, new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Secondary", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.i.ReturnPosition(new Vector3(0.1f, 0.85f)), new Vector3(-1.5f,-5f) }
        });

        MonoBehaviour[] test = UIDrawer.i.Spawn("Text").o;
        UIDrawer.i.SetUIComponent(test,typeof(Image), null);
    }

    // Update is called once per frame
    void Update() {
        

    }
}

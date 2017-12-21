using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStorer : MonoBehaviour {

    public UIElement[] ui;

    void Start () {
        UIDrawer.i.InitialiseNewUIList(ui);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawer : MonoBehaviour {

    public List<UIElement> uE;
    public static UIDrawer i;

    void Start() {
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < uE.Count; i++) {
            if (uE[i].u.gameObject.activeSelf)
                if (uE[i].dAT <= Time.time)
                    uE[i].u.gameObject.SetActive(false);
        }
    }

    public void UpdateGraphic(string n, string c, float t) {
        UIElement uII = uE[BaseIteratorFunctions.IterateKey(uE.ToArray(), n)];
        Text tI = (Text)uII.u;
        tI.text = c;
        uII.u.gameObject.SetActive(true);

        uII.dAT = Time.time + t;
    }

    public void UpdateGraphic(string n, Sprite c, float t) {

    }

    public void InitialiseNewUIList(UIElement[] pV) {
        uE = new List<UIElement>(pV);
    }
}

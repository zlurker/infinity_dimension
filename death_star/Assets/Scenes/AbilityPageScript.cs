using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AbilityPageScript : MonoBehaviour {

    ScriptableObject lL;
    // Use this for initialization
    void Start() {
        GenerateMenuElements();
    }

    void GenerateMenuElements() {
        ScriptableObject topText = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(Text) });
        Spawner.GetCType<Text>(topText).text = "Abilities";
        Spawner.GetCType<Text>(topText).color = Color.white;

        topText.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.5f, 0.9f));

        ScriptableObject addAbility = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(Button) });
        Spawner.GetCType<Button>(addAbility).onClick.AddListener(() => { CreateAbility(); });
        Spawner.GetCType<Text>(addAbility).text = "Create Ability";

        addAbility.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.15f, 0.1f));

        lL = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(LinearLayout) });
        lL.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.15f, 0.75f));
    }

    void CreateAbility() {
        string bPath = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).fP;
        string path = "";
        int i = 0;

        do {
            path = Path.Combine(bPath,i.ToString());
            i++;
        } while(Directory.Exists(path));

        Directory.CreateDirectory(path);

        Spawner.GetCType<LinearLayout>(lL).Add(GenerateAbilityElement(path).transform as RectTransform);
    }

    ScriptableObject GenerateAbilityElement(string fp) {
        //ScriptableObject background = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(Image) });
        ScriptableObject abilityButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(Button) });
        Spawner.GetCType<Button>(abilityButton).onClick.AddListener(() => {

        });

        UIDrawer.ChangeUISize(abilityButton, new Vector2(200, 30));
        return abilityButton;
    }

    // Update is called once per frame
    void Update() {

    }
}

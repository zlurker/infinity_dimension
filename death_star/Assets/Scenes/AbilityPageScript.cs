using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AbilityPageScript : MonoBehaviour {

    ScriptableObject lL;
    public static int selectedAbility;
    // Use this for initialization
    void Start() {
        GenerateMenuElements();
        LoadCurrentFiles();
    }

    void LoadCurrentFiles() {
        FileSaveTemplate fST = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; });
        DirectoryInfo dir = Directory.CreateDirectory(fST.fP);

        DirectoryInfo[] files = dir.GetDirectories();

        for (int i=0; i < files.Length; i++) 
            GenerateAbilityElement(int.Parse(files[i].Name));
        
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
        FileSaveTemplate fST = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; });
        string path = "";
        int i = -1;

        do {
            i++;
            path = Path.Combine(fST.fP, i.ToString());

        } while(Directory.Exists(path));

        fST.GenerateNewSubDirectory(new string[] { i.ToString() });
        GenerateAbilityElement(i);
    }

    void GenerateAbilityElement(int index) {
        ScriptableObject abilityButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new System.Type[] { typeof(Button) });
        Spawner.GetCType<Button>(abilityButton).onClick.AddListener(() => {
            selectedAbility = index;
            SceneTransitionData.LoadScene("Lobby");
        });

        UIDrawer.ChangeUISize(abilityButton, new Vector2(200, 30));
        Spawner.GetCType<LinearLayout>(lL).Add(abilityButton.transform as RectTransform);
    }
}

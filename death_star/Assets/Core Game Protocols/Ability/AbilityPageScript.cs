using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class AbilityPageScript : MonoBehaviour {

    SpawnerOutput lL;
    public static int selectedAbility;
    AutoPopulationList<AbilityDescription> descriptions;

    // Use this for initialization
    void Start() {
        descriptions = new AutoPopulationList<AbilityDescription>();
        GenerateMenuElements();
        LoadCurrentFiles();

        LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollRect));
    }

    void LoadCurrentFiles() {
        DirectoryInfo dir = Directory.CreateDirectory(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP);

        DirectoryInfo[] files = dir.GetDirectories();

        for(int i = 0; i < files.Length; i++) {
            int fileInt = int.Parse(files[i].Name);

            string data = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { fileInt.ToString() }, 1);
            AbilityDescription inst = JsonConvert.DeserializeObject<AbilityDescription>(data);

            descriptions.ModifyElementAt(fileInt, inst);
            
            GenerateAbilityElement(fileInt);         
        }
    }

    void GenerateMenuElements() {
        SpawnerOutput topText = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        UIDrawer.GetTypeInElement<Text>(topText).text = "Abilities";
        UIDrawer.GetTypeInElement<Text>(topText).color = Color.white;

        topText.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.5f, 0.9f));

        SpawnerOutput addAbility = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        UIDrawer.GetTypeInElement<Button>(addAbility).onClick.AddListener(() => { CreateAbility(); });
        UIDrawer.GetTypeInElement<Text>(addAbility).text = "Create Ability";

        addAbility.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.15f, 0.1f));

        lL = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));
        lL.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.15f, 0.75f));
    }

    void CreateAbility() {
        FileSaveTemplate fST = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA];
        string path = "";
        int i = -1;

        do {
            i++;
            path = Path.Combine(fST.fP, i.ToString());
        } while(Directory.Exists(path));

        fST.GenerateNewSubDirectory(new string[] { i.ToString() });

        AbilityDescription inst = new AbilityDescription();
        fST.GenericSaveTrigger<string>(new string[] { i.ToString() }, 1, JsonConvert.SerializeObject(inst));
        
        descriptions.ModifyElementAt(i, inst);
        GenerateAbilityElement(i);
    }

    void GenerateAbilityElement(int index) {
        SpawnerOutput abilityButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));

        UIDrawer.GetTypeInElement<Text>(abilityButton).text = descriptions.GetElementAt(index).n;

        UIDrawer.GetTypeInElement<Button>(abilityButton).onClick.AddListener(() => {
            selectedAbility = index;
            SceneTransitionData.LoadScene("Lobby");
        });

        //UIDrawer.ChangeUISize(abilityButton, new Vector2(200, 30));
        UIDrawer.GetTypeInElement<LinearLayout>(lL).Add(abilityButton.script.transform as RectTransform);
    }
}

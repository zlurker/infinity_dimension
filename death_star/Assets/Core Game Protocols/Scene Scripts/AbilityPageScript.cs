﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public enum AbilityManifest {
    PRIMARY_CHARACTER
}

public class AbilityPageScript : MonoBehaviour {

    enum AbilityButtonMode {
        DEFAULT, CHANGE_PRIMARY_CHARACTER
    }

    public static Dictionary<string, AbilityInfo> abilityInfo;
    public static string selectedAbility;

    SpawnerOutput lL;
    SpawnerOutput commandText;

    Dictionary<int, string> abilityManifest;

    AbilityButtonMode currMode;

    string abilityManifestPath;

    // Use this for initialization
    void Start() {

        if(abilityInfo == null) {
            abilityInfo = new Dictionary<string, AbilityInfo>();
            LoadCurrentFiles();
        }

        GenerateMenuElements();
        GenerateElementFromInfo();
        LoadAbilityManifest();
    }


    void LoadCurrentFiles() {
        DirectoryInfo dir = Directory.CreateDirectory(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP);

        DirectoryInfo[] files = dir.GetDirectories();

        for(int i = 0; i < files.Length; i++) {
            int fileInt = int.Parse(files[i].Name);

            string data = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { fileInt.ToString() }, 1);
            AbilityInfo inst = JsonConvert.DeserializeObject<AbilityInfo>(data);

            if(!abilityInfo.ContainsKey(files[i].Name))
                abilityInfo.Add(files[i].Name, inst);
            //aInfo.ModifyElementAt(fileInt, inst);
        }
    }

    void GenerateElementFromInfo() {
        foreach(var abilityEle in abilityInfo)
            GenerateAbilityElement(abilityEle.Key);
    }

    void LoadAbilityManifest() {

        abilityManifestPath = Path.Combine(FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].fP, "AbilityManifest.json");

        if(File.Exists(abilityManifestPath)) {
            string fileContents = File.ReadAllText(abilityManifestPath);
            abilityManifest = JsonConvert.DeserializeObject<Dictionary<int, string>>(fileContents);
        } else
            abilityManifest = new Dictionary<int, string>();
    }

    void SaveAbilityManifest() {
        string contents = JsonConvert.SerializeObject(abilityManifest);
        File.WriteAllText(abilityManifestPath, contents);
    }

    void GenerateMenuElements() {
        SpawnerOutput topText = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(topText).text = "Abilities";
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(topText).color = Color.white;

        topText.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.5f, 0.9f));

        commandText = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(commandText).text = "";
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(commandText).color = Color.white;

        commandText.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.5f, 0.75f));

        SpawnerOutput addAbility = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(addAbility).onClick.AddListener(() => { CreateAbility(); });
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(addAbility, "Text").text = "Create Ability";

        addAbility.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.15f, 0.1f));

        SpawnerOutput setNewPrimary = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(setNewPrimary).onClick.AddListener(() => { ChangeButtonMode(AbilityButtonMode.CHANGE_PRIMARY_CHARACTER); });
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(setNewPrimary, "Text").text = "Set new primary";

        setNewPrimary.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.9f, 0.8f));

        lL = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));
        lL.script.transform.position = UIDrawer.UINormalisedPosition(new Vector2(0.1f, 0.75f));
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

        AbilityInfo inst = new AbilityInfo();
        fST.GenericSaveTrigger<string>(new string[] { i.ToString() }, 1, JsonConvert.SerializeObject(inst));

        abilityInfo.Add(i.ToString(), inst);
        //aInfo.ModifyElementAt(i, inst);
        GenerateAbilityElement(i.ToString());
    }

    void GenerateAbilityElement(string index) {
        SpawnerOutput abilityButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(abilityButton, "Text").text = abilityInfo[index].n;

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(abilityButton).onClick.AddListener(() => {
            selectedAbility = index;

            switch(currMode) {
                case AbilityButtonMode.CHANGE_PRIMARY_CHARACTER:
                    if(!abilityManifest.ContainsKey((int)AbilityManifest.PRIMARY_CHARACTER))
                        abilityManifest.Add((int)AbilityManifest.PRIMARY_CHARACTER, index);
                    else
                        abilityManifest[(int)AbilityManifest.PRIMARY_CHARACTER] = index;

                    SaveAbilityManifest();
                    break;
                case AbilityButtonMode.DEFAULT:
                    SceneTransitionData.LoadScene("AbilityMaker");
                    break;
            }

            ChangeButtonMode(AbilityButtonMode.DEFAULT);
        });

        //UIDrawer.ChangeUISize(abilityButton, new Vector2(200, 30));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(lL).Add(abilityButton.script.transform as RectTransform);
    }

    void ChangeButtonMode(AbilityButtonMode mode) {
        currMode = mode;

        switch(currMode) {
            case AbilityButtonMode.CHANGE_PRIMARY_CHARACTER:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(commandText).text = "Select ability to be new primary character.";
                break;
            default:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(commandText).text = "";
                break;
        }
    }
}

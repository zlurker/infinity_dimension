using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CommonUIUtilities : MonoBehaviour {
}

public class KeyCodeDropdownList {

    public static List<Dropdown.OptionData> inputNames;
    public static int[] inputValues;

    public DropdownWrapper dW;

    public KeyCodeDropdownList(int keycode) {

        PopulateValues();

        dW = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper)).script as DropdownWrapper;       
        dW.dropdown.AddOptions(inputNames);

        for(int i = 0; i < inputValues.Length; i++)
            if(keycode == inputValues[i])
                dW.dropdown.value = i;
    }

    void PopulateValues() {
        if(inputNames == null) {
            string[] inputStrs = Enum.GetNames(typeof(KeyCode));
            inputNames = new List<Dropdown.OptionData>();

            for(int i = 0; i < inputStrs.Length; i++) 
                inputNames.Add(new Dropdown.OptionData(inputStrs[i]));
        }

        if(inputValues == null)
            inputValues = (int[])Enum.GetValues(typeof(KeyCode));
    }
}
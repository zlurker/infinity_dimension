﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSpawner : AbilityTreeNode, IOnSpawn {

    public static Transform backgroundLayer;
    protected SpriteRenderer sR;

    public override void NodeCallback() {
        base.NodeCallback();

        if(backgroundLayer == null)
            backgroundLayer = GameObject.Find("Background").transform;

        Sprite givenSprite = AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>("Sprite Name")];

        if(!GetNodeVariable<bool>("Don't Load Sprite into Object"))
            sR.sprite = givenSprite;

        if(!GetNodeVariable<bool>("Background"))
            sR.sortingLayerName = "Default";

        Color spriteColor = sR.color;
        spriteColor.a = GetNodeVariable<float>("Object Transperency");

        sR.color = spriteColor;
        SetVariable<Sprite>("Sprite", givenSprite);
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
        int o = GetVariableId("Sprite Name");

        if(o == variable) {
            SpawnerOutput oField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Dropdown>(oField);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<string> rpI = rp as RuntimeParameters<string>;
            int ddId = 0;
            var imagePaths = new DirectoryInfo(Path.Combine(LoadedData.gameDataPath, "UsrCreatedArt")).GetFiles().Where(x => x.Extension != ".meta");

            foreach(FileInfo fI in imagePaths) {
                if(fI.Name == rpI.v)
                    ddId = dOd.Count;

                dOd.Add(new Dropdown.OptionData(fI.Name));
                //Od.Add(new Dropdown.OptionData("Actual Position"));
            }

            dW.AddOptions(dOd);
            dW.value = ddId;

            dW.onValueChanged.AddListener((id) => {
                rpI.v = dOd[id].text;
            });

            return oField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite Name",""), VariableTypes.AUTO_MANAGED,VariableTypes.IMAGE_DEPENDENCY),
            new LoadedRuntimeParameters(new RuntimeParameters<Sprite>("Sprite",null)),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Don't Load Sprite into Object", false)),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Background", false)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Object Transperency",1))
        });
    }

    public virtual void OnSpawn() {
        if(sR == null)
            sR = GetComponent<SpriteRenderer>();
    }
}

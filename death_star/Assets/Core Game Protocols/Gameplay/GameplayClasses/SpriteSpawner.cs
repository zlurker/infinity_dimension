using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class SpriteSpawner : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();
        SetVariable<Sprite>("Sprite", AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>("Sprite Name")]);
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
        int o = GetVariableId("Sprite Name");

        if(o == variable) {
            SpawnerOutput oField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = ((oField.script as DropdownWrapper).mainScript as Dropdown);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<string> rpI = rp as RuntimeParameters<string>;
            int ddId = 0;
            var imagePaths = new DirectoryInfo(Path.Combine(Application.dataPath, "UsrCreatedArt")).GetFiles().Where(x => x.Extension != ".meta");

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
            new LoadedRuntimeParameters(new RuntimeParameters<Sprite>("Sprite",null))
        });
    }
}

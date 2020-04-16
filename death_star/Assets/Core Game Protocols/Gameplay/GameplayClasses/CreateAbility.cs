using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int aN = GetVariableId("Ability Name");

        if (variable == aN) {
            SpawnerOutput aNField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = ((aNField.script as DropdownWrapper).mainScript as Dropdown);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();

            foreach (var kPV in AbilityPageScript.abilityInfo) 
                dOd.Add(new Dropdown.OptionData(kPV.Value.n));

            dW.AddOptions(dOd);

            dW.onValueChanged.AddListener((id) => {
                string[] dirNames = AbilityPageScript.abilityInfo.Keys.ToArray();
                (rp as RuntimeParameters<string>).v = dirNames[id];
            });

            return aNField;
        }

        return null;
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<string>("Ability Name", "")));
    }
}

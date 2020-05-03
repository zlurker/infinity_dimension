using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        AbilityCentralThreadPool inst = GetCentralInst();

        AbilityCentralThreadPool newA = new AbilityCentralThreadPool(inst.GetPlayerId());
        //Debug.Log(GetNodeVariable<string>("Ability Name"));
        AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA, ClientProgram.clientId, -1, inst.GetClusterID());
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int aN = GetVariableId("Ability Name");

        if(variable == aN) {
            SpawnerOutput aNField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = ((aNField.script as DropdownWrapper).mainScript as Dropdown);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<string> rpS = rp as RuntimeParameters<string>;
            int selected = 0;

            foreach(var kPV in AbilityPageScript.abilityInfo) {
                dOd.Add(new Dropdown.OptionData(kPV.Value.n));

                if(kPV.Key == rpS.v)
                    selected = dOd.Count - 1;
            }

            dW.AddOptions(dOd);

            dW.value = selected;

            dW.onValueChanged.AddListener((id) => {
                string[] dirNames = AbilityPageScript.abilityInfo.Keys.ToArray();
                rpS.v = dirNames[id];
            });

            return aNField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<string>("Ability Name", ""), VariableTypes.HOST_ACTIVATED));
    }
}

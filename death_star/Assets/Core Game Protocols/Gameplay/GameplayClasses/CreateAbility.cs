using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        AbilityCentralThreadPool inst = GetCentralInst();

        AbilityCentralThreadPool newA = new AbilityCentralThreadPool(inst.GetPlayerId());
        //Debug.Log(GetNodeVariable<string>("Ability Name"));
        AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA,inst.GetClusterID());
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int aN = GetVariableId("Ability Name");

        if(variable == aN) {
            SpawnerOutput aNField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = ((aNField.script as DropdownWrapper).mainScript as Dropdown);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            int selected = 0;

            foreach(var kPV in AbilityPageScript.abilityInfo) {
                dOd.Add(new Dropdown.OptionData(kPV.Value.n));

                if (kPV.Key == (rp as RuntimeParameters<string>).v) 
                    selected = dOd.Count - 1;
            }

            dW.AddOptions(dOd);

            dW.value = selected;

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

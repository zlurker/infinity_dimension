using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Operator : AbilityTreeNode {

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int o = GetVariableId("Operation");

        if(o == variable) {
            SpawnerOutput oField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = ((oField.script as DropdownWrapper).mainScript as Dropdown);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<int> rpI = rp as RuntimeParameters<int>;

            dOd.Add(new Dropdown.OptionData("A + B"));
            dOd.Add(new Dropdown.OptionData("A - B"));
            dOd.Add(new Dropdown.OptionData("A * B"));
            dOd.Add(new Dropdown.OptionData("A / B"));

            dW.AddOptions(dOd);

            dW.value = rpI.v;

            dW.onValueChanged.AddListener((id) => {
                rpI.v = id;
            });

            return oField;
        }

        return null;
    }


    public override void NodeCallback(int threadId) {
        if(CheckIfVarRegionBlocked(0, 1)) {

            float output = 0;

            switch(GetNodeVariable<int>("Operation")) {
                case 0:
                    output = GetNodeVariable<float>("A") + GetNodeVariable<float>("B");
                    break;

                case 1:
                    output = GetNodeVariable<float>("A") - GetNodeVariable<float>("B");
                    break;

                case 2:
                    output = GetNodeVariable<float>("A") * GetNodeVariable<float>("B");
                    break;

                case 3:
                    output = GetNodeVariable<float>("A") / GetNodeVariable<float>("B");
                    break;
            }

            SetVariable<float>("Output", output);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Operation",0), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("A",0), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("B",0), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Output",0))
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conditionals : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(CheckIfVarRegionBlocked("A", "B")) {

            bool conditionFulfilled = false;

            switch(GetNodeVariable<int>("Condition")) {
                case 0:
                    if(GetNodeVariable<float>("A") > GetNodeVariable<float>("B"))
                        conditionFulfilled = true;
                    break;

                case 1:
                    if(GetNodeVariable<float>("A") < GetNodeVariable<float>("B"))
                        conditionFulfilled = true;
                    break;

                case 2:
                    if(GetNodeVariable<float>("A") == GetNodeVariable<float>("B"))
                        conditionFulfilled = true;
                    break;

                case 3:
                    if(GetNodeVariable<float>("A") >= GetNodeVariable<float>("B"))
                        conditionFulfilled = true;
                    break;

                case 4:
                    if(GetNodeVariable<float>("A") <= GetNodeVariable<float>("B"))
                        conditionFulfilled = true;
                    break;
            }

            //Debug.LogFormat("Operation: {0}, A: {1}, B: {2}, Output: {3}", GetNodeVariable<int>("Operation"), GetNodeVariable<float>("A"), GetNodeVariable<float>("B"), output);

            if(conditionFulfilled) {
                //Debug.Log("Condition fulfilled");
                SetVariable<int>("On Condition Fulfilled", 0);
            }
        }
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int o = GetVariableId("Condition");

        if(o == variable) {
            SpawnerOutput oField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Dropdown>(oField);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<int> rpI = rp as RuntimeParameters<int>;

            dOd.Add(new Dropdown.OptionData("A is greater than B"));
            dOd.Add(new Dropdown.OptionData("A is lesser than B"));
            dOd.Add(new Dropdown.OptionData("A is equal to B"));
            dOd.Add(new Dropdown.OptionData("A is greater than or equal to B"));
            dOd.Add(new Dropdown.OptionData("A is lesser than or equal to B"));

            dW.AddOptions(dOd);

            dW.value = rpI.v;

            dW.onValueChanged.AddListener((id) => {
                rpI.v = id;
            });

            return oField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Condition", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("A",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("B",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("On Condition Fulfilled",0),VariableTypes.SIGNAL_ONLY),
        });
    }
}

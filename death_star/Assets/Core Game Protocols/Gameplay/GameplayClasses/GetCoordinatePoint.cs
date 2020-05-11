using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCoordinatePoint : AbilityTreeNode {

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int p = GetVariableId("Point");

        if (p == variable) {
            SpawnerOutput pField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Dropdown>(pField);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<int> rpI = rp as RuntimeParameters<int>;

            dOd.Add(new Dropdown.OptionData("X"));
            dOd.Add(new Dropdown.OptionData("Y"));

            dW.AddOptions(dOd);

            dW.value = rpI.v;

            dW.onValueChanged.AddListener((id) => {
                rpI.v = id;
            });

            return pField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void NodeCallback() {
        base.NodeCallback();

        if(CheckIfVarRegionBlocked("Coordinate")) {
            //Debug.Log("Point value is: " + GetNodeVariable<Vector3>("Coordinate")[GetNodeVariable<int>("Point")]);
            SetVariable<float>("Point Value", GetNodeVariable<Vector3>("Coordinate")[GetNodeVariable<int>("Point")]);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Coordinate",new Vector3()),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Point",0), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Point Value", 0))
        });

    }
}

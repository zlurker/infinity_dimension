using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputType : AbilityTreeNode {

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        if(variable == GetVariableId("Input Key")) {
            KeyCodeDropdownList kcDdL = new KeyCodeDropdownList((rp as RuntimeParameters<int>).v);

            kcDdL.ReturnDropdownWrapper().dropdown.onValueChanged.AddListener((v) => {
                (rp as RuntimeParameters<int>).v = KeyCodeDropdownList.inputValues[v];
            });

            return kcDdL.dW;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0),VariableTypes.AUTO_MANAGED)
            //new LoadedRuntimeParameters(new RuntimeParameters<int>("Curr", 0), VariableTypes.HIDDEN),
            //new LoadedRuntimeParameters(new RuntimeParameters<int>("Max", 0), VariableTypes.CLIENT_ACTIVATED,VariableTypes.HIDDEN)
        });
    }
}

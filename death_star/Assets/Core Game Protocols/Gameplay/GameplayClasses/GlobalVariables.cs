using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : AbilityTreeNode, IRPGeneric {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Variable Name","Default Name"), VariableTypes.GLOBAL_VARIABLE, VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Variable Value",0),VariableTypes.INTERCHANGEABLE, VariableTypes.AUTO_MANAGED, VariableTypes.BLOCKED)
        });
    }

    public override void NodeCallback() {

        int[] nodeId = AbilitiesManager.GetAssetData(GetCastingPlayerId()).globalVariables[GetNodeVariable<string>("Variable Name")];
        Debug.Log(nodeId[0] + " " + nodeId[1]);

        if (reference != null) {

        }
        SetVariable<AbilityTreeNode>("This Node", AbilitiesManager.GetAssetData(nodeId[0]).playerSpawnedCentrals.l[0].GetNode(nodeId[1]));

        //base.NodeCallback();

        if (CheckIfVarRegionBlocked("Variable Value")) 
            GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Variable Value")).field.RunGenericBasedOnRP<int>(this, 0);       
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        SetVariable<T>("Variable Value");
    }
}

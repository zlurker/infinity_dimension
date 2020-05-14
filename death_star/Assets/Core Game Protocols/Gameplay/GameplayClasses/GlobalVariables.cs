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

        if(!GetCentralInst().CheckIfReferenced(GetNodeId(), GetVariableId("Variable Value"))) {

            int[] nodeId = AbilitiesManager.GetAssetData(GetCastingPlayerId()).globalVariables[GetNodeVariable<string>("Variable Name")];

            if(!(nodeId[0] == GetCentralInst().ReturnPlayerCasted() && 0 == GetCentralId() && nodeId[1] == GetNodeId())) {
                Debug.Log(name + " has been instanced.");
                AbilityTreeNode globalVarSource = AbilitiesManager.GetAssetData(nodeId[0]).playerSpawnedCentrals.l[0].GetNode(nodeId[1]);
                InstanceThisNode(globalVarSource);
            }
        }

        if(CheckIfVarRegionBlocked("Variable Value")) {
            GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Variable Value")).field.RunGenericBasedOnRP<int>(this, 0);
            Debug.Log("Variable was sent out.");
        }
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        SetVariable<T>("Variable Value");
    }
}

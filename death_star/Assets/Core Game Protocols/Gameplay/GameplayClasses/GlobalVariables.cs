using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalVariables : AbilityTreeNode, IRPGeneric, IOnNodeInitialised {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Variable Name","Default Name"), VariableTypes.GLOBAL_VARIABLE, VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Variable Value",0),VariableTypes.INTERCHANGEABLE, VariableTypes.AUTO_MANAGED, VariableTypes.BLOCKED)
        });
    }

    public override void NodeCallback() {
        if(CheckIfVarRegionBlocked("Variable Value"))
            GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Variable Value")).field.RunGenericBasedOnRP<int>(this, 0);
    }

    public void OnNodeInitialised() {
        Tuple<int,int,int> nodeId = AbilitiesManager.GetAssetData(GetCentralInst().GetPlayerId()).globalVariables[GetNodeVariable<string>("Variable Name")];

        // Handles callback from subnodes.
        if(!(nodeId.Item1 == GetCentralInst().ReturnPlayerCasted() && nodeId.Item2 == GetCentralId() && nodeId.Item3 == GetNodeId())) {
            if(!GetCentralInst().CheckIfReferenced(GetNodeId(), GetVariableId("Variable Value"))) {
                //Debug.Log(nodeId[1]);
                GetCentralInst().InstanceNode(GetNodeId(), nodeId);
                //AbilityTreeNode globalVarSource = AbilitiesManager.GetAssetData(nodeId[0]).playerSpawnedCentrals.l[0].GetNode(nodeId[1]);
                //InstanceThisNode(globalVarSource);
            }
        }
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        SetVariable<T>("Variable Value");
    }
}

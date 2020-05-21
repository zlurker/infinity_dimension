using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloningModule : ModuleBase {

    public override void NodeCallback() {
        base.NodeCallback();

        AbilityTreeNode sN = GetNodeVariable<AbilityTreeNode>("Source Node");
        Variable tNVar = GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Target Nodes"));

        for(int i = 0; i < tNVar.links.Length; i++)
            GetCentralInst().CopyNodeVariables(tNVar.links[i][0], sN.GetCentralInst().ReturnPlayerCasted(), sN.GetCentralId(), sN.GetNodeId());

        SetVariable<int>("Target Nodes");
    }
}

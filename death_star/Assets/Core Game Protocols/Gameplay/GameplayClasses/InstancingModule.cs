using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstancingModule : ModuleBase {

    public override void NodeCallback() {
        base.NodeCallback();

        AbilityTreeNode sN = GetNodeVariable<AbilityTreeNode>("Source Node");
        Variable tNVar = GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Target Nodes"));
        Tuple<int, int, int> refNode = Tuple.Create<int, int, int>(sN.GetCentralInst().ReturnPlayerCasted(), sN.GetCentralId(), sN.GetNodeId());

        for(int i = 0; i < tNVar.links.Length; i++)
            GetCentralInst().InstanceNode(tNVar.links[i][0], refNode);

        SetVariable<int>("Target Nodes");
    }
}

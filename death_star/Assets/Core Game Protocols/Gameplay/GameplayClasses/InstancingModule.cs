using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstancingModule : ModuleBase {

    public override void NodeCallback() {
        base.NodeCallback();

        
        Tuple<int, int, int> refNode = Tuple.Create<int, int, int>(sourceNode.GetCentralInst().ReturnPlayerCasted(), sourceNode.GetCentralId(), sourceNode.GetNodeId());

        for(int i = 0; i < targetNodeVar.links.Length; i++)
            GetCentralInst().InstanceNode(targetNodeVar.links[i][0], refNode);

        SetVariable<int>("Target Nodes");
    }
}

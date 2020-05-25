using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstancingModule : ModuleBase {

    public override void NodeCallback() {
        base.NodeCallback();
    
        Tuple<int, int, int> refNode = Tuple.Create<int, int, int>(sourceNode.GetCentralInst().ReturnPlayerCasted(), sourceNode.GetCentralId(), sourceNode.GetNodeId());

        for(int i = 0; i < links.Length; i++)
            GetCentralInst().InstanceNode(links[i][0], refNode);

        SetVariable<int>("Target Nodes");
    }
}

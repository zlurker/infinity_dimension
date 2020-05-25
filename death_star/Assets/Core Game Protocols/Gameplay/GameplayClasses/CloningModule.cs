using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloningModule : ModuleBase {

    public override void NodeCallback() {
        base.NodeCallback();

        for(int i = 0; i < links.Length; i++)
            GetCentralInst().CopyNodeVariables(links[i][0], sourceNode.GetCentralInst().ReturnPlayerCasted(), sourceNode.GetCentralId(), sourceNode.GetNodeId());

        SetVariable<int>("Target Nodes");
    }
}

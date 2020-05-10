using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeModifierLooper : AbilityTreeNode {
    

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        //data.Get data.GetCurrBuildNode()
    }

    public void AddNetworkData(AbilityNodeNetworkData dataPacket) {

    }

    public void SendNetworkData() {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : AbilityTreeNode {

    public override void NodeCallback(int threadId) {
        TravelThread central = TravelThread.globalCentralList.l[GetCentralId()];

        Vector3 movement = new Vector3(central.ReturnRuntimeParameter<float>(GetNodeId(), 1).v, central.ReturnRuntimeParameter<float>(GetNodeId(), 2).v);
        central.ReturnRuntimeParameter<GameObject>(GetNodeId(), 0).v.transform.position += movement;
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<GameObject>("Move Target", null),
            new RuntimeParameters<float>("X", 0),
            new RuntimeParameters<float>("Y", 0)       
        };
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : AbilityTreeNode {

    public override void NodeCallback(int threadId, int nodeId) {
        AbilityCentralThreadPool central = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        //central.SyncDataWithNetwork<GameObject>(GetNodeThreadId(), 1, GameObject.Find(central.ReturnRuntimeParameter<string>(GetNodeId(), 0).v));
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("ID","0"),
            new RuntimeParameters<GameObject>("Object",null)
        };
    }
}

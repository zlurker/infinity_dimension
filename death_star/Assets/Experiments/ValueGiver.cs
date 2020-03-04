using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueGiver : AbilityTreeNode {

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Testvalue", "")
        };
    }

    public override void NodeCallback(int threadId) {
        AbilityCentralThreadPool central = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        //central.SyncDataWithNetwork<string>(threadId, 0, central.ReturnRuntimeParameter<string>(GetNodeId(), 0).v);
    }
}

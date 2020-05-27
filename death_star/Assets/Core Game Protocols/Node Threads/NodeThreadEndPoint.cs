using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeThreadEndPoint : AbilityTreeNode {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        //base.GetRuntimeParameters(holder);

        holder.Add(new LoadedRuntimeParameters(new RuntimeParameters<int>("", 0)));
    }

    public override void NodeCallback() {
    }

}

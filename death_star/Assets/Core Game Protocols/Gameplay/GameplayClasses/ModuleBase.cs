using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleBase : AbilityTreeNode {

    protected AbilityTreeNode sourceNode;
    protected Variable targetNodeVar;

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<AbilityTreeNode>("Source Node", null), VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Target Nodes",0), VariableTypes.PERMENANT_TYPE, VariableTypes.SIGNAL_ONLY)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        AbilityTreeNode srcNodeInst = GetNodeVariable<AbilityTreeNode>("Source Node");
        sourceNode = srcNodeInst.GetCentralInst().GetRootReferenceNode(srcNodeInst.GetNodeId());
        targetNodeVar = GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Target Nodes"));
    }
}

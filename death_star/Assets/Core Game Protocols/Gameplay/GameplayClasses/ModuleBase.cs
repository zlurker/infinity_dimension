using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleBase : AbilityTreeNode {

    protected AbilityTreeNode sourceNode;
    //protected RuntimeParameters targetNodeVar;
    protected int[][] links;

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
        //Debug.Log(GetCentralInst().ReturnRuntimeParameter(GetNodeId(), "Source Node").n);
        //Debug.Log("srcNI" + srcNodeInst);
        sourceNode = srcNodeInst.GetCentralInst().GetRootReferenceNode(srcNodeInst.GetNodeId());
        links = GetCentralInst().GetVariableLinks(0, GetNodeId(), GetVariableId("Target Nodes"));
        //targetNodeVar = GetCentralInst().ReturnRuntimeParameter(GetNodeId(), GetVariableId("Target Nodes"));
    }
}

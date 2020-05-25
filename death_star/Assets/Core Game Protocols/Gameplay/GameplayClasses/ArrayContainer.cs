using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayContainer : AbilityTreeNode, IRPGeneric {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Values", 0),VariableTypes.PERMENANT_TYPE, VariableTypes.SIGNAL_ONLY, VariableTypes.NON_LINK),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Array Element",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Output from Array",0), VariableTypes.INTERCHANGEABLE)
        });
    }

    public override void NodeCallback() {
        base.NodeCallback();

        int[] arrayValues = GetCentralInst().GetVariableLinks(0,GetNodeId(), GetVariableId("Values"))[GetNodeVariable<int>("Array Element")];
        RuntimeParameters targetVar = GetCentralInst().ReturnRuntimeParameter(arrayValues[0], arrayValues[1]);
        targetVar.RunGenericBasedOnRP<RuntimeParameters>(this, targetVar);
    }

    public void RunAccordingToGeneric<T, P>(P arg) {

        RuntimeParameters<T> targetVar = (RuntimeParameters<T>)(object)arg;
        SetVariable<T>("Output from Array", targetVar.v);
    }
}

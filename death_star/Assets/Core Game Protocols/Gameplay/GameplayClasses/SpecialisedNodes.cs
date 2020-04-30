using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningData : ThreadMapDataBase {

    public int node;
    public int variable;


    public ReturningData(int n, int v) {
        node = n;
        variable = v;
    }
}

public class SpecialisedNodes : NodeModifierBase, IRPGeneric {

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        foreach(var t1 in data.GetLinkData(data.GetCurrBuildNode()).lHS)
            if(t1.Item2 == GetVariableId("Extended Path")) {
                data.AddTargettedNode(t1.Item1, t1.Item2, GetType(), data.GetCurrBuildNode());
                Debug.LogFormat("Built: {0},{1}", t1.Item1, t1.Item2);
                data.GetLinkModifier().Remove(t1.Item1, t1.Item2, t1.Item3);
            }
    }

    // Creates returning data and helps us to create a child thread based on what was given.
    public virtual int CentralCallback<T>(T value, int nodeId, int varId) {
        AbilityCentralThreadPool inst = GetCentralInst();
        int transferThread = inst.GetNewThread(GetNodeId());
        threadMap.Add(transferThread, new ReturningData(nodeId, varId));

        ChildThread cT = new ChildThread(GetNodeId(), transferThread, this);
        return inst.AddNewThread(cT);
    }

    public override void ThreadZeroed(int parentThread) {
        base.ThreadZeroed(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        if(centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links.Length == 0)
            return;

        int[] modifiedReturn = centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        centralInst.ReturnVariable(modifiedReturn[0], modifiedReturn[1]).field.RunGenericBasedOnRP<int>(this, parentThread);
        threadMap.Remove(parentThread);
    }

    public virtual void RunAccordingToGeneric<T, P>(P arg) {

    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY, VariableTypes.NON_LINK),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Internal Redirect",0), VariableTypes.HIDDEN, VariableTypes.INTERCHANGEABLE)
        });
    }

}

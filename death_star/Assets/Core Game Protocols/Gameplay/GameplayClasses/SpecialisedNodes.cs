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

public interface IOnVariableInterface {
    int CentralCallback<T>(T value, int nodeId, int varId, int links);
}

public class SpecialisedNodes : NodeModifierBase, IOnVariableInterface, IRPGeneric {

    protected RuntimeParameters returnTargetInst;


    // Creates returning data and helps us to create a child thread based on what was given.
    public virtual int CentralCallback<T>(T value, int nodeId, int varId, int links = 0) {

        //Debug.Log("Central base called.");

        AbilityCentralThreadPool inst = GetCentralInst();
        int transferThread = inst.GetNewThread();
        threadMap.Add(transferThread, new ReturningData(nodeId, varId));

        if(links > 0) {
            NodeModifierBaseThread cT = new NodeModifierBaseThread(transferThread, this);
            cT.SetNodeData(GetNodeId(), links);
            return inst.AddNewThread(cT);
        } else
            ThreadZeroed(transferThread);

        return -1;
    }

    public override void ThreadZeroed(int parentThread) {
        base.ThreadZeroed(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        if(centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links.Length == 0)
            return;

        int[] modifiedReturn = centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links[0];
        Variable varInst = centralInst.ReturnVariable(modifiedReturn[0], modifiedReturn[1]);

        returnTargetInst = varInst.field;
        varInst.field.RunGenericBasedOnRP<int>(this, parentThread);
        threadMap.Remove(parentThread);
    }

    public virtual void RunAccordingToGeneric<T, P>(P arg) {

    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY, VariableTypes.NON_LINK),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Internal Redirect",0), VariableTypes.HIDDEN, VariableTypes.INTERCHANGEABLE)
        });
    }

}

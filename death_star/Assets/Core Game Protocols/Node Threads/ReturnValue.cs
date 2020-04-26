using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnValue : NodeModifierBase, IRPGeneric {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Internal Redirect",0), VariableTypes.HIDDEN,VariableTypes.INTERCHANGEABLE)
        });
    }

    public override void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

        foreach(var t1 in linkData[id].lHS) {
            foreach(var t2 in linkData[t1.Item1].lHS) {
                lM.Add(t2.Item1, t2.Item2, id, 0,1);

                int[] links = var[t2.Item1][t2.Item2].links[t2.Item4];

                if(links[1] == t1.Item2) 
                    lM.Remove(t2.Item1, t2.Item2, t2.Item4);
            }

            // Removes main connection from the list.
            lM.Remove(t1.Item1, t1.Item2, t1.Item4);
            //Debug.LogFormat("Added {0}, {1}, {2} to return", t1.Item1, t1.Item2, t1.Item3);

            // Adds it to internal redirect.
            lM.Add(id, 3, t1.Item1, t1.Item2, t1.Item3);
        }
    }

    public override void NodeCallback() {
        base.NodeCallback();
        //Debug.Log("afterset TID: " + threadId);

        threadMap.Add(latestThread, new ThreadMapDataBase());

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), latestThread, this);

        trdInst.SetNodeData(GetNodeId(), inst.ReturnVariable(GetNodeId(), "Extended Path").links.Length);
        int threadToUse = inst.AddNewThread(trdInst);
        //Debug.LogFormat("Thread id {0} has been created. Uses left {1}", threadToUse, inst.ReturnVariable(GetNodeId(), "Extended Path").links.Length);
        inst.NodeVariableCallback<int>(threadToUse,GetVariableId("Extended Path"));
    }

    public override void ThreadZeroed(int parentThread) {
        AbilityCentralThreadPool inst = GetCentralInst();

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        inst.ReturnVariable(varToReturn[0], varToReturn[1]).field.RunGenericBasedOnRP(this, parentThread);
        threadMap.Remove(parentThread);
    }


    public void RunAccordingToGeneric<T, P>(P arg) {
        AbilityCentralThreadPool inst = GetCentralInst();

        int parentThread = (int)(object)arg;

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        RuntimeParameters<T> rP = inst.ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);

        inst.GetActiveThread(parentThread).SetPossiblePaths(inst.ReturnVariable(GetNodeId(), "Internal Redirect").links.Length);

        Debug.LogFormat("Redirecting variable now. Variable is: {0}. Number of Links: {1}.",rP.v ,inst.ReturnVariable(GetNodeId(), "Internal Redirect").links.Length);
        Debug.Log("currnode:" + inst.GetActiveThread(parentThread).GetCurrentNodeID());


        SetVariable(parentThread,"Internal Redirect", rP.v);
    }
}
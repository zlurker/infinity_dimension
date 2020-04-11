using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnNodeData : ThreadMapDataBase {

    public int node;
    public int vSource;
    public ReturnNodeData(int n, int vSrc) {
        node = n;
        vSource = vSrc;
    }
}

public class ReturnValue : NodeModifierBase, IRPGeneric {

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Internal Redirect",0), VariableTypes.HIDDEN)
        });
    }

    public override void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

        // Finds second cell first before performing any actions.
        foreach(var t1 in linkData[id].lHS) {
            foreach(var t2 in linkData[t1.Item1].lHS) {
                lM.Add(t2.Item1, t2.Item2, id, 0);

                int[] links = var[t2.Item1][t2.Item2].links[t2.Item3];

                if(links[1] == t1.Item2)
                    lM.Remove(t2.Item1, t2.Item2, t2.Item3);
            }

            // Removes main connection from the list.
            lM.Remove(t1.Item1, t1.Item2, t1.Item3);
            lM.Add(id, 2, t1.Item1, t1.Item2);
        }
    }

    public override void PreSetCallback(int threadId) {
        Debug.Log("Preset TID: " + threadId);
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);
        int node = nT.GetCurrentNodeID() > -1 ? nT.GetCurrentNodeID() : nT.GetStartingPoint();

        threadMap.Add(threadId, new ReturnNodeData(node, inst.GetActiveThread(threadId).GetVariableSource()));
    }

    public override void NodeCallback(int threadId) {
        Debug.Log("afterset TID: " + threadId);
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), threadId, this);

        int falseGeneratedLinks = inst.ReturnVariable(GetNodeId(), "Return from Variable").links.Length;

        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()) - falseGeneratedLinks);
        int threadToUse = inst.AddNewThread(trdInst);
        Debug.LogFormat("Thread id {0} has been created. Uses left {1}", threadToUse, inst.GetNodeBranchData(GetNodeId()) - falseGeneratedLinks);
        inst.NodeVariableCallback<int>(threadToUse, "Extended Path", 0);
    }

    public override void ThreadZeroed(int parentThread) {
        AbilityCentralThreadPool inst = GetCentralInst();
        ReturnNodeData returnNodeData = threadMap[parentThread] as ReturnNodeData;

        inst.ReturnVariable(returnNodeData.node, returnNodeData.vSource).field.RunGenericBasedOnRP(this, parentThread);
        threadMap.Remove(parentThread);
    }


    public void RunAccordingToGeneric<T, P>(P arg) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        int parentThread = (int)(object)arg;

        ReturnNodeData rData = threadMap[parentThread] as ReturnNodeData;
        int overridenNode = rData.node;
        int vSource = rData.vSource;

        int[][] overridenLinks = inst.ReturnVariable(overridenNode, vSource).links[0];

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), "Return from Variable").links[0][0];

        RuntimeParameters<T> rP = inst.ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);

        inst.GetActiveThread(parentThread).SetLinksData(overridenLinks);
        inst.GetActiveThread(parentThread).SetVariableSource(vSource);
        inst.GetActiveThread(parentThread).SetNodeData(overridenNode, overridenLinks.Length);
        inst.UpdateVariableData<T>(parentThread, rP.v);
    }
}
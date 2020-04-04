using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnValue : AbilityTreeNode, IRPGeneric {

    public const int EXTENDED_PATH = 0;
    public const int RETURN_FROM_VARIABLE = 1;
    Dictionary<int, int> threadMap = new Dictionary<int, int>();

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY)
        };
    }

    public override void NodeCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), threadId);
        threadMap.Add(threadId, 0);

        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

        int threadToUse = inst.AddNewThread(trdInst);
        Debug.LogFormat("Thread id {0} has been created.", threadToUse);
        inst.NodeVariableCallback<int>(threadToUse, EXTENDED_PATH, 0);
    }

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        if(nT is ChildThread) {

            int parentThread = (nT as ChildThread).GetOriginalThread();

            threadMap[parentThread] += 1;

            Debug.LogFormat("Thread id {0}, current node collection progress {1}/{2}", threadId, threadMap[parentThread], inst.GetSpecialisedNodeData(GetNodeId()));

            if(threadMap[parentThread] >= inst.GetSpecialisedNodeData(GetNodeId())) {
                int nS = inst.GetActiveThread(parentThread).GetNodeSource();
                int vS = inst.GetActiveThread(parentThread).GetVariableSource();

                inst.ReturnVariable(nS, vS).field.RunGenericBasedOnRP(this, parentThread);
            }
        }
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        int overridenNode = inst.GetActiveThread((int)(object)arg).GetNodeSource();
        int vSource = inst.GetActiveThread((int)(object)arg).GetVariableSource();
        int[][] overridenLinks = inst.GetOverridenConnections(overridenNode,vSource);

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), RETURN_FROM_VARIABLE).links[0];

        RuntimeParameters<T> rP = inst.ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        inst.UpdateVariableData<T>((int)(object)arg, overridenLinks, rP.v);
    }
}
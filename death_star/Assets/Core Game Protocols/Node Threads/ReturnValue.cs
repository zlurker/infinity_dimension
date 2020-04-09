using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnValue : AbilityTreeNode, IRPGeneric, ISubNode {

    public const int EXTENDED_PATH = 0;
    public const int RETURN_FROM_VARIABLE = 1;

    Dictionary<int, int[]> threadMap = new Dictionary<int, int[]>();

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY)
        };
    }

    public override void PreSetCallback(int threadId) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);
        int node = nT.GetCurrentNodeID() > -1 ? nT.GetCurrentNodeID() : nT.GetStartingPoint();

        threadMap.Add(threadId, new int[] { node, inst.GetActiveThread(threadId).GetVariableSource(), 0 });
    }

    public override void NodeCallback(int threadId) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), threadId, this);

        int falseGeneratedLinks = inst.ReturnVariable(GetNodeId(), RETURN_FROM_VARIABLE).links.Length;

        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()) - falseGeneratedLinks);
        int threadToUse = inst.AddNewThread(trdInst);
        Debug.Log(threadId);
        Debug.LogFormat("Thread id {0} has been created. Uses left {1}", threadToUse, inst.GetNodeBranchData(GetNodeId()) - falseGeneratedLinks);
        inst.NodeVariableCallback<int>(threadToUse, EXTENDED_PATH, 0);
    }

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        if(nT is ChildThread) {

            int parentThread = (nT as ChildThread).GetOriginalThread();

            threadMap[parentThread][2]--;

            Debug.LogFormat("Thread id {0}, current node collection progress {1}", threadId, threadMap[parentThread][2]);

            if(threadMap[parentThread][2] == 0) {
                int nS = threadMap[parentThread][0];
                int vS = threadMap[parentThread][1];

                inst.ReturnVariable(nS, vS).field.RunGenericBasedOnRP(this, parentThread);

                //Debug.LogFormat("Thread id {0} will end.", parentThread);
                //inst.HandleThreadRemoval(parentThread);

                if(threadMap.Count == 0) {
                    Debug.Log("Threadmap empty. Setting node thread id to -1.");
                    SetNodeThreadId(-1);
                }
            }
        }
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];

        int parentThread = (int)(object)arg;
        int overridenNode = threadMap[parentThread][0];
        int vSource = threadMap[parentThread][1];
         
        int[][] overridenLinks = inst.GetOverridenConnections(overridenNode, vSource);

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), RETURN_FROM_VARIABLE).links[0];

        RuntimeParameters<T> rP = inst.ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        
        inst.GetActiveThread(parentThread).SetLinksData(overridenLinks);
        inst.GetActiveThread(parentThread).SetVariableSource(vSource);
        inst.GetActiveThread(parentThread).SetNodeData(overridenNode,overridenLinks.Length);
        inst.UpdateVariableData<T>(parentThread, rP.v);
    }

    public void AddThread(int oT) {
        threadMap[oT][2]++;
    }


}
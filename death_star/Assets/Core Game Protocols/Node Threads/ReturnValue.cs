using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnValue : AbilityTreeNode {

    public class InternalGenericHolder<T> {

    }

    Dictionary<int, int> threadMap = new Dictionary<int, int>();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public override LoadedRuntimeParameters[] GetRuntimeParameters() {
        return new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path", 0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0))
        };
    }

    public override void NodeCallback(int threadId) {


        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        ChildThread trdInst = new ChildThread(GetNodeId(), threadId);
        threadMap.Add(threadId, 0);

        trdInst.SetNodeData(GetNodeId(), inst.GetNodeBranchData(GetNodeId()));

        int threadToUse = inst.AddNewThread(trdInst);
        Debug.LogFormat("Thread id {0} has been created.", threadToUse);
        inst.NodeVariableCallback<int>(threadToUse, 0,0);
    }

    public override void ThreadEndStartCallback(int threadId) {

        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[GetCentralId()];
        NodeThread nT = inst.GetActiveThread(threadId);

        if(nT is ChildThread) {

            int parentThread = (nT as ChildThread).GetOriginalThread();

            threadMap[parentThread] += 1;

            Debug.LogFormat("Thread id {0}, current node collection progress {1}/{2}", threadId, threadMap[parentThread], inst.GetSpecialisedNodeData(GetNodeId()));

            if(threadMap[parentThread] >= inst.GetSpecialisedNodeData(GetNodeId())) {
                // Return value of target.
                //Variable storedAddress = inst.ReturnVariable(GetNodeId(), 1);

                int overridenNode = inst.GetActiveThread(parentThread).GetCurrentNodeID();
                int[][] overridenLinks = inst.GetOverridenConnections(overridenNode);

                inst.UpdateVariableData<>
                if(storedAddress.links.Length > 0) {
                    int[] latestAddress = storedAddress.links[storedAddress.links.Length - 1];
                    Debug.Log("Variable returned: " + (inst.ReturnRuntimeParameter<int>(latestAddress[0], latestAddress[1]) as RuntimeParameters<int>).v);
                }
                //storedAddress.links[1][storedAddress.links[1].Length-1][0]
                //
            }
        }
    }
}
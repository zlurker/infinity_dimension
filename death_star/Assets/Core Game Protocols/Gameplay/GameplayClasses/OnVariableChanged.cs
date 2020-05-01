using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnVariableChanged : SpecialisedNodes, IRPGeneric {


    /*public override void NodeCallback() {
        base.NodeCallback();

        AbilityCentralThreadPool centralInst = GetCentralInst();

        int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;

        for(int i = 0; i < links.Length; i++) {
            AbilityTreeNode originatorNode = GetCentralInst().GetNode(links[i][0]);

            bool added = GetCentralInst().GetRootReferenceCentral(links[i][0]).AddOnChanged(Tuple.Create<int, int>(originatorNode.GetReference().Item2, links[i][1]), Tuple.Create<int, int>(GetCentralId(), GetNodeId()));

            if(added)
                recievedThreads.Add(latestThread);
            else
                GetCentralInst().HandleThreadRemoval(latestThread);
        }
    }*/

    /*public void HandleSettingOnChange<T>(T[] valuePair, params int[] centralId) {

        /*int dictKey = -1;

        if(recievedThreads.Count > 0) {
            dictKey = recievedThreads[recievedThreads.Count - 1];
            recievedThreads.RemoveAt(recievedThreads.Count - 1);
        } else {
            Debug.Log("Overpromised! Not enough threads");
            return;
        }

        threadMap.Add(dictKey, new OnChangeDataBase(centralId));
        ChildThread cT = new ChildThread(GetNodeId(), dictKey, this);

        AbilityCentralThreadPool inst = GetCentralInst();

        int totalLinks = inst.ReturnVariable(GetNodeId(), "Old Value").links.Length + inst.ReturnVariable(GetNodeId(), "New Value").links.Length;
        cT.SetNodeData(GetNodeId(), totalLinks);
        int threadToUse = inst.AddNewThread(cT);

        SetVariable<T>(threadToUse, "Old Value", valuePair[0]);
        SetVariable<T>(threadToUse, "New Value", valuePair[1]);
    }*/

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        Debug.Log("Construction phase called. LHS Links: " + data.GetLinkData(data.GetCurrBuildNode()).lHS.Count);
        foreach(var t1 in data.GetLinkData(data.GetCurrBuildNode()).lHS) {

            //Debug.LogFormat("Connected var id: {0}. Curr var needed: {1}. Curr node: {2}", data.GetVariable(t1.Item1, t1.Item2).links[t1.Item3][1], GetVariableId("Extended Path"), data.GetCurrBuildNode());

            if(data.GetVariable(t1.Item1, t1.Item2).links[t1.Item3][1] == GetVariableId("Old Value") || data.GetVariable(t1.Item1, t1.Item2).links[t1.Item3][1] == GetVariableId("New Value")) {
                data.AddTargettedNode(t1.Item1, t1.Item2, GetType(), data.GetCurrBuildNode());
                Debug.LogFormat("Built {0}: {1},{2}", GetType(), t1.Item1, t1.Item2);
                data.GetLinkModifier().Remove(t1.Item1, t1.Item2, t1.Item3);
            }
        }
    }

    public override int CentralCallback<T>(T value, int nodeId, int varId) {
        

        int childThread = base.CentralCallback(value, nodeId, varId);
        NodeThread cTInst = GetCentralInst().GetActiveThread(childThread);

        int totalLinks = GetCentralInst().ReturnVariable(GetNodeId(), "Old Value").links.Length + GetCentralInst().ReturnVariable(GetNodeId(), "New Value").links.Length;
        Debug.Log(totalLinks);
        cTInst.SetNodeData(GetNodeId(), totalLinks);

        SetVariable<T>(childThread, "Old Value", GetCentralInst().ReturnRuntimeParameter<T>(nodeId,varId).v);
        SetVariable<T>(childThread, "New Value", value);
        return childThread;
    }

    /* override void ThreadZeroed(int parentThread) {
        /*base.ThreadZeroed(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        //int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;
        int[] modifiedReturn = centralInst.ReturnVariable(GetNodeId(), "Modified Value To Return").links[0];

        //for(int i = 0; i < links.Length; i++) {
        OnChangeDataBase oCDB = threadMap[parentThread] as OnChangeDataBase;
        //int[] idParams = new int[] { links[i][0], links[i][1] };
        centralInst.ReturnVariable(modifiedReturn[0], modifiedReturn[1]).field.RunGenericBasedOnRP<int[]>(this, oCDB.centralId);
        //}

        GetCentralInst().HandleThreadRemoval(parentThread);
        threadMap.Remove(parentThread);
    }*/

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Old Value",0),VariableTypes.INTERCHANGEABLE),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("New Value",0),VariableTypes.INTERCHANGEABLE)
        });
    }

    public override void RunAccordingToGeneric<T, P>(P arg) {

        int parentThread = (int)(object)arg;
        ReturningData oCDB = threadMap[parentThread] as ReturningData;
        //AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[oCDB.];

        RuntimeParameters<T> rP = returnTargetInst as RuntimeParameters<T>;
        //Debug.Log("Returning central " + inst);
        Debug.LogFormat("Returning modified variable {0} ", rP.v);
  
        GetCentralInst().UpdateVariableValue<T>(oCDB.node, oCDB.variable, rP.v, false);
    }
}

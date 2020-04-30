using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class OnValueChange : NodeModifierBase, IRPGeneric {

    List<int> recievedThreads = new List<int>();

    public override void ConstructionPhase(AbilityData data) {


        // Might need rewriting. 
        //foreach(var t1 in data.GetLinkData(data.GetCurrBuildNode()).lHS)

            // Adds all those who called this node into empty link storage.
            //lM.Add(id, 4, t1.Item1, t1.Item2, t1.Item3);
    }

    public override void NodeCallback() {
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
    }

    public void HandleSettingOnChange<T>(T[] valuePair, params int[] centralId) {

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
        SetVariable<T>(threadToUse, "New Value", valuePair[1]);*/
    }

    public override void ThreadZeroed(int parentThread) {
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
        threadMap.Remove(parentThread);*/
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Old Value",0),VariableTypes.INTERCHANGEABLE),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("New Value",0),VariableTypes.INTERCHANGEABLE),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Modified Value To Return",0),VariableTypes.INTERCHANGEABLE),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Empty link storage",0), VariableTypes.HIDDEN)
        });
    }

    public void RunAccordingToGeneric<T, P>(P arg) {

        int[] idParams = (int[])(object)arg;
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[idParams[0]];

        int[] varToReturn = GetCentralInst().ReturnVariable(GetNodeId(), "Modified Value To Return").links[0];

        RuntimeParameters<T> rP = GetCentralInst().ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        //Debug.Log("Returning central " + inst);
        Debug.LogFormat("Returning modified variable {0} to id: {1},{2} ", rP.v, idParams[1], idParams[2]);
        inst.UpdateVariableValue<T>(idParams[1], idParams[2], rP.v, false);
    }
}

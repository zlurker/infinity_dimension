﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnValueChange : NodeModifierBase, IRPGeneric {

    public override void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

        foreach(var t1 in linkData[id].lHS)

            // Adds all those who called this node into empty link storage.
            lM.Add(id, 4, t1.Item1, t1.Item2, t1.Item3);
    }

    public override void NodeCallback() {
        base.NodeCallback();

        AbilityCentralThreadPool centralInst = GetCentralInst();

        int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;

        for(int i = 0; i < links.Length; i++) {
            AbilityTreeNode originatorNode = GetCentralInst().GetNode(links[i][0]);
            //Tuple<int,int> linkTup = originatorNode.GetReference();

            //originatorNode = globalList.l[linkTup.Item1].abiNodes[linkTup.Item2];

            //Debug.LogFormat("Link added: {0}, Id1: {1}, Id2: {2}", linkTup, GetCentralId(),GetNodeId());

            //AbilityCentralThreadPool rootCentral

            GetCentralInst().GetRootReferenceCentral(links[i][0]).AddOnChanged(Tuple.Create<int, int>(originatorNode.GetReference().Item2, links[i][1]), Tuple.Create<int, int>(GetCentralId(), GetNodeId()));
            //originatorNode.GetInstanceCentralInst().AddOnChanged(originatorNode.GetReference(),Tuple.Create<int,int>(GetCentralId(),GetNodeId()));
        }
    }

    public void HandleSettingOnChange<T>(T[] valuePair) {

        if (GetNodeThreadId() > -1) {
            threadMap.Add(GetNodeThreadId(), new ThreadMapDataBase());
            ChildThread cT = new ChildThread(GetNodeId(), GetNodeThreadId(), this);

            AbilityCentralThreadPool inst = GetCentralInst();

            int totalLinks = inst.ReturnVariable(GetNodeId(), "Old Value").links.Length + inst.ReturnVariable(GetNodeId(), "New Value").links.Length;
            cT.SetNodeData(GetNodeId(), totalLinks);
            int threadToUse = inst.AddNewThread(cT);

            SetVariable<T>(threadToUse,"Old Value", valuePair[0]);
            SetVariable<T>(threadToUse,"New Value", valuePair[1]);
        }
    }

    public override void ThreadZeroed(int parentThread) {
        base.ThreadZeroed(parentThread);
        threadMap.Remove(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;
        int[] modifiedReturn = centralInst.ReturnVariable(GetNodeId(), "Modified Value To Return").links[0];

        for(int i = 0; i < links.Length; i++) {
            int[] idParams = new int[] { links[i][0],links[i][1] };

            centralInst.ReturnVariable(modifiedReturn[0],modifiedReturn[1]).field.RunGenericBasedOnRP<int[]>(this, idParams);
        }       
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
        AbilityCentralThreadPool inst = GetCentralInst();

        int[] varToReturn = inst.ReturnVariable(GetNodeId(), "Modified Value To Return").links[0];

        RuntimeParameters<T> rP = inst.ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        //Debug.Log("Returning central " + inst);
        Debug.LogFormat("Returning modified variable {0} to id: {1},{2} ", rP.v, idParams[0], idParams[1]);
        inst.UpdateVariableValue<T>(idParams[0], idParams[1], rP.v,false);
    }
}

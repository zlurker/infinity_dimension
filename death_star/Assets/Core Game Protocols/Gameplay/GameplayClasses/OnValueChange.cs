using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnValueChange : NodeModifierBase, IRPGeneric {

    public override void LinkEdit(int id, LinkData[] linkData, LinkModifier lM, Variable[][] var) {

        foreach(var t1 in linkData[id].lHS)

            // Adds all those who called this node into empty link storage.
            lM.Add(id, 3, t1.Item1, t1.Item2, t1.Item3);
    }

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;

        for(int i = 0; i < links.Length; i++) {
            AbilityTreeNode originatorNode = globalList.l[GetCentralId()].abiNodes[links[i][0]];
            Tuple<int,int> linkTup = originatorNode.GetReference();

            if(linkTup == null) 
                linkTup = Tuple.Create<int, int>(GetCentralId(), links[i][0]);
            else
                originatorNode = globalList.l[linkTup.Item1].abiNodes[linkTup.Item2];

            originatorNode.GetCentralInst().AddOnChanged(linkTup,Tuple.Create<int,int>(GetCentralId(),GetNodeId()));
        }
    }

    public override void ThreadZeroed(int parentThread, int lastChildThread) {
        base.ThreadZeroed(parentThread, lastChildThread);
        threadMap.Remove(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;

        for(int i = 0; i < links.Length; i++) {
            Tuple<int, int> linkTup = globalList.l[GetCentralId()].abiNodes[links[i][0]].GetReference();
            int[] idParams = new int[] { GetCentralId(),links[i][0],links[i][1] };

            if(linkTup != null) {
                idParams[0] = linkTup.Item1;
                idParams[1] = linkTup.Item2;
            }

            centralInst.ReturnVariable(GetNodeId(), "Modified Value To Return").field.RunGenericBasedOnRP<int[]>(this, idParams);
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
        AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[idParams[0]];

        int[] varToReturn = GetCentralInst().ReturnVariable(GetNodeId(), "Modified Value To Return").links[0];

        RuntimeParameters<T> rP = GetCentralInst().ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        Debug.Log("Returning modified variable.");
        inst.UpdateVariableValue<T>(idParams[1], idParams[2], rP.v,false);
    }
}

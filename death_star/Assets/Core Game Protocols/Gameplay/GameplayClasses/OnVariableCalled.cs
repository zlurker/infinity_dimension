using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnVariableCalled : SpecialisedNodes {

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        Debug.Log("Construction phase called. LHS Links: " + data.GetLinkData(data.GetCurrBuildNode()).lHS.Count);
        foreach(var t1 in data.GetLinkData(data.GetCurrBuildNode()).lHS) {

            Debug.LogFormat("Connected var id: {0}. Curr var needed: {1}. Curr node: {2}", data.GetVariable(t1.Item1, t1.Item2).links[t1.Item3][1], GetVariableId("Extended Path"), data.GetCurrBuildNode());

            if(data.GetVariable(t1.Item1, t1.Item2).links[t1.Item3][1] == GetVariableId("Extended Path")) {
                data.AddTargettedNode(t1.Item1, t1.Item2, GetType(), data.GetCurrBuildNode());
                Debug.LogFormat("Built {0}: {1},{2}", GetType(), t1.Item1, t1.Item2);
                data.GetLinkModifier().Remove(t1.Item1, t1.Item2, t1.Item3);
            }
        }
    }

    public override void CentralCallback<T>(T value, int nodeId, int varId, params string[] vars) {
        //Debug.Log("Central Called");
        base.CentralCallback(value, nodeId, varId, "Extended Path");
    }

    public override void RunAccordingToGeneric<T, P>(P arg) {
        int parentThread = (int)(object)arg;
        ReturningData oCDB = threadMap[parentThread] as ReturningData;
        //AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[idParams[0]];

        int[] varToReturn = GetCentralInst().ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        RuntimeParameters<T> rP = GetCentralInst().ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        //Debug.Log("Returning central " + inst);
        //Debug.LogFormat("Returning modified variable {0} to id: {1},{2} ", rP.v, idParams[1], idParams[2]);
        //Debug.LogFormat("Returning to {0},{1}", oCDB.node, oCDB.variable);
        //GetCentralInst().GetNode(oCDB.centralId[0]).SetVariable<T>(oCDB.centralId[1], rP.v);

        //GetCentralInst().GetActiveThread(parentThread).SetNodeData(GetNodeId(), 1);

        //AbilityCentralThreadPool poolInst = AbilityCentralThreadPool.globalCentralList.l[oCDB.centralId[0]];

        // Manually sets variable and callback original node.
        //poolInst.SetNodeBoolValue(false, oCDB.centralId[1], oCDB.centralId[2]);
        //poolInst.UpdateVariableValue<T>(oCDB.centralId[1], oCDB.centralId[2], rP.v,false);
        //poolInst.GetNode(oCDB.centralId[1]).NodeCallback();

        GetCentralInst().GetActiveThread(parentThread).SetNodeData(GetNodeId(), 1);
        GetCentralInst().ReturnVariable(GetNodeId(), GetVariableId("Internal Redirect")).links = new int[][] { new int[] { oCDB.node, oCDB.variable, 0 } };
        GetCentralInst().UpdateVariableValue<T>(GetNodeId(), GetVariableId("Internal Redirect"), rP.v);
        GetCentralInst().UpdateVariableData<T>(parentThread, GetVariableId("Internal Redirect"), null, false);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path",0))
        });
    }
}


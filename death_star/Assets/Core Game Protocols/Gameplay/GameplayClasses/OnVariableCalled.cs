using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnVariableCalled : NodeModifierBase, IRPGeneric {

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        foreach(var t1 in data.GetLinkData(data.GetCurrBuildNode()).lHS)
            if(t1.Item2 == GetVariableId("Extended Path")) {
                data.AddOnCalled(t1.Item1, t1.Item2, data.GetCurrBuildNode());
                Debug.LogFormat("Built: {0},{1}", t1.Item1,t1.Item2);
                data.GetLinkModifier().Remove(t1.Item1, t1.Item2, t1.Item3);
            }      
    }

    public void OnVariableCalledCallback<T>(T value, int threadId, params int[] nVInfo) {
        Debug.Log("Called with a value of " + value);
        threadMap.Add(threadId, new OnChangeDataBase(nVInfo));
        ChildThread cT = new ChildThread(GetNodeId(), threadId, this);

        AbilityCentralThreadPool inst = GetCentralInst();

        int totalLinks = inst.ReturnVariable(GetNodeId(), "Extended Path").links.Length;
        cT.SetNodeData(GetNodeId(), totalLinks);
        int threadToUse = inst.AddNewThread(cT);

        SetVariable<int>(threadToUse,"Extended Path");
    }

    // Only called by another OnVariableCalled
    public override void NodeCallback() {
        base.NodeCallback();


    }

    public override void ThreadZeroed(int parentThread) {
        base.ThreadZeroed(parentThread);

        AbilityCentralThreadPool centralInst = GetCentralInst();

        //int[][] links = centralInst.ReturnVariable(GetNodeId(), "Empty link storage").links;

        if(centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links.Length == 0)
            return;

        int[] modifiedReturn = centralInst.ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        //for(int i = 0; i < links.Length; i++) {

        //int[] idParams = new int[] { links[i][0], links[i][1] };
        centralInst.ReturnVariable(modifiedReturn[0], modifiedReturn[1]).field.RunGenericBasedOnRP<int>(this, parentThread);
        //}

        GetCentralInst().HandleThreadRemoval(parentThread);
        threadMap.Remove(parentThread);
    }

    public void RunAccordingToGeneric<T, P>(P arg) {

        int parentThread = (int)(object)arg;
        OnChangeDataBase oCDB = threadMap[parentThread] as OnChangeDataBase;
        //AbilityCentralThreadPool inst = AbilityCentralThreadPool.globalCentralList.l[idParams[0]];

        int[] varToReturn = GetCentralInst().ReturnVariable(GetNodeId(), "Return from Variable").links[0];

        RuntimeParameters<T> rP = GetCentralInst().ReturnRuntimeParameter<T>(varToReturn[0], varToReturn[1]);
        //Debug.Log("Returning central " + inst);
        //Debug.LogFormat("Returning modified variable {0} to id: {1},{2} ", rP.v, idParams[1], idParams[2]);
        Debug.LogFormat("Returning to {0},{1}", oCDB.centralId[0], oCDB.centralId[1]);
        //GetCentralInst().GetNode(oCDB.centralId[0]).SetVariable<T>(oCDB.centralId[1], rP.v);
        //GetCentralInst().UpdateVariableValue<T>(oCDB.centralId[0], oCDB.centralId[1], rP.v, false);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Extended Path",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Return from Variable", 0),VariableTypes.PERMENANT_TYPE,VariableTypes.SIGNAL_ONLY, VariableTypes.NON_LINK)
        });
    }
}

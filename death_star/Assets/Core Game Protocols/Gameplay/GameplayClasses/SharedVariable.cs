using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableInterfaces: IRPGeneric {

    public class InternalValueHolder<T> : InternalValueHolder {
        public T value;
    }

    public class InternalValueHolder {

    }

    public List<SharedVariable> sVList;
    public InternalValueHolder valueHolder;
    

    public VariableInterfaces() {
        sVList = new List<SharedVariable>();
    }

    public void Callback(SharedVariable caller) {
        AbilityCentralThreadPool inst = caller.GetCentralInst();
        inst.ReturnVariable(caller.GetNodeId(), caller.GetVariableId("Variable Value")).field.RunGenericBasedOnRP<SharedVariable>(this,caller);
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        AbilityTreeNode node = arg as AbilityTreeNode;

        AbilityCentralThreadPool inst = node.GetCentralInst();
        T value = inst.ReturnRuntimeParameter<T>(node.GetNodeId(), node.GetVariableId("Variable Value")).v;

        for(int i = 0; i < sVList.Count; i++)
            sVList[i].SetVariable<T>("Variable Value", value);

        InternalValueHolder<T> vhInst = valueHolder as InternalValueHolder<T>;

        if(vhInst == null)
            vhInst = new InternalValueHolder<T>();

        vhInst.value = value;
    }
}

public class SharedVariable : AbilityTreeNode, IOnSpawn {

    public static Dictionary<int, Dictionary<string,VariableInterfaces>> sharedVariables = new Dictionary<int, Dictionary<string, VariableInterfaces>>();

    public override void NodeCallback(int threadId) {
        base.NodeCallback(threadId);

        if (CheckIfVarRegionBlocked(GetVariableId("Variable Value"))) 
            for (int i =0; i < GetVariableInterface().sVList.Count; i++) 
                GetVariableInterface().Callback(this);        
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Variable Name",""),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Variable Value",0),VariableTypes.INTERCHANGEABLE)
        });
    }

    public void OnSpawn() {
        GetVariableInterface().sVList.Add(this);
    }

    VariableInterfaces GetVariableInterface() {

        int clusterRootId = AbilityCentralThreadPool.globalCentralClusterList.l[GetCentralInst().GetClusterID()][0];

        if(!sharedVariables.ContainsKey(clusterRootId))
            sharedVariables.Add(clusterRootId, new Dictionary<string, VariableInterfaces>());

        if(!sharedVariables[clusterRootId].ContainsKey(GetNodeVariable<string>("Variable Name")))
            sharedVariables[clusterRootId].Add(GetNodeVariable<string>("Variable Name"), new VariableInterfaces());

        return sharedVariables[clusterRootId][GetNodeVariable<string>("Variable Name")];
    }
}

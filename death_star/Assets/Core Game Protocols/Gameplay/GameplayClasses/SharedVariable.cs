using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableInterfaces : IRPGeneric {

    public class InternalValueHolder<T> : InternalValueHolder {
        public T value;

        public override void ForceSetValue(SharedVariable target) {
            //Debug.Log("Variable has been forceset.");
            target.SetVariable<T>("Variable Value", value);
        }
    }

    public class InternalValueHolder {

        public virtual void ForceSetValue(SharedVariable target) {

        }
    }

    public List<SharedVariable> sVList;
    public InternalValueHolder valueHolder;

    public VariableInterfaces() {
        sVList = new List<SharedVariable>();
    }

    public void AddReference(SharedVariable caller) {
        sVList.Add(caller);

        if(valueHolder != null)
            valueHolder.ForceSetValue(caller);
    }

    public void Callback(SharedVariable caller) {
        AbilityCentralThreadPool inst = caller.GetCentralInst();

        inst.ReturnVariable(caller.GetNodeId(), caller.GetVariableId("Variable Value")).field.RunGenericBasedOnRP<SharedVariable>(this, caller);
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        SharedVariable node = arg as SharedVariable;

        AbilityCentralThreadPool inst = node.GetCentralInst();
        InternalValueHolder<T> vhInst = valueHolder as InternalValueHolder<T>;
        T value = inst.ReturnRuntimeParameter<T>(node.GetNodeId(), node.GetVariableId("Variable Value")).v;

        for(int i = 0; i < sVList.Count; i++)
            sVList[i].SetVariable<T>("Variable Value", value);

        if(vhInst == null)
            valueHolder = new InternalValueHolder<T>();

        (valueHolder as InternalValueHolder<T>).value = value;
    }
}


public class SharedVariable : AbilityTreeNode {

    bool referenceAdded;

    public override void NodeCallback() {
        base.NodeCallback();

        if(!referenceAdded) {
            GetVariableInterface().AddReference(this);
            referenceAdded = true;
        }

        if(CheckIfVarRegionBlocked("Variable Value"))
            GetVariableInterface().Callback(this);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Variable Name",""),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<bool>("Global Variable",false),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Variable Value",0),VariableTypes.INTERCHANGEABLE, VariableTypes.BLOCKED)
        });
    }

    VariableInterfaces GetVariableInterface() {

        int clusterRootId = 0;

        if (GetNodeVariable<bool>("Global Variable")) 
            clusterRootId = -1;
        else
            clusterRootId = AbilityCentralThreadPool.globalCentralClusterList.l[GetCentralInst().GetClusterID()][0];

        if(!AbilitiesManager.aData[GetCentralInst().GetPlayerId()].globalVariables.ContainsKey(clusterRootId))
            AbilitiesManager.aData[GetCentralInst().GetPlayerId()].globalVariables.Add(clusterRootId, new Dictionary<string, VariableInterfaces>());

        if(!AbilitiesManager.aData[GetCentralInst().GetPlayerId()].globalVariables[clusterRootId].ContainsKey(GetNodeVariable<string>("Variable Name")))
            AbilitiesManager.aData[GetCentralInst().GetPlayerId()].globalVariables[clusterRootId].Add(GetNodeVariable<string>("Variable Name"), new VariableInterfaces());

        return AbilitiesManager.aData[GetCentralInst().GetPlayerId()].globalVariables[clusterRootId][GetNodeVariable<string>("Variable Name")];
    }
}
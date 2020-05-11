using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class ThreadDuplicatorTracker: ThreadMapDataBase {
    public int threadsDuplicated;

    public ThreadDuplicatorTracker(int totalThreadsDuplicated) {
        threadsDuplicated = totalThreadsDuplicated;
    }
}*/

public class ClientInput : AbilityTreeNode, IInputCallback<int>, IOnSpawn, IOnVariableInterface {

    bool inputSet;

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

       //Debug.Log("Construction phase called. LHS Links: " + data.GetLinkData(data.GetCurrBuildNode()).lHS.Count);
       data.AddTargettedNode(data.GetCurrBuildNode(), GetVariableId("Internal Input Track"), ON_VARIABLE_CATERGORY.ON_CHANGED, data.GetCurrBuildNode());
    }

    public int CentralCallback<T>(T value, int nodeId, int varId, int links) {

        //SetVariable<bool>("Internal Input Track", (bool)(object)value);

        if(GetNodeThreadId() > -1)
            if((bool)(object)value)
                TriggerInput();

        return 0;
    }

    public void InputCallback(int callbackData) {
        inputSet = false;
        //Debug.Log("Input called");
        SetVariable<bool>("Internal Input Track", true);
        //SetVariable<int>("Input Key");    
    }

    public override void NodeCallback() {
        base.NodeCallback();

        if(GetNodeVariable<bool>("Internal Input Track"))
            TriggerInput();

        if(IsClientPlayerUpdate()) {
            if(!inputSet)
                LoadedData.GetSingleton<PlayerInput>().AddNewInput<int>(this, 0, (KeyCode)GetNodeVariable<int>("Input Key"), 1);

            inputSet = true;
        }
    }

    void TriggerInput() {

        // Unsets input triggered.
        GetCentralInst().UpdateVariableValue<bool>(GetNodeId(), GetVariableId("Internal Input Track"), false, false);
        SetVariable<int>("Input Key");
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Input Key", 0), VariableTypes.SIGNAL_ONLY),
             new LoadedRuntimeParameters(new RuntimeParameters<bool>("Internal Input Track", false), VariableTypes.HIDDEN, VariableTypes.CLIENT_ACTIVATED)
            //new LoadedRuntimeParameters(new RuntimeParameters<int>("Curr", 0), VariableTypes.HIDDEN),
            //new LoadedRuntimeParameters(new RuntimeParameters<int>("Max", 0), VariableTypes.CLIENT_ACTIVATED,VariableTypes.HIDDEN)
        });
    }

    public void OnSpawn() {
        inputSet = false;
    }


}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : AbilityTreeNode, ITimerCallback, IRPGeneric {

    int eventId = -1;

    public void CallOnTimerEnd(int eventId) {
        SetVariable<float>("Duration");
        GetCentralInst().ReturnRuntimeParameter(GetNodeId(),GetVariableId("Value to Pass")).RunGenericBasedOnRP(this, 0);
    }

    public void RunAccordingToGeneric<T, P>(P arg) {
        SetVariable<T>("Value to Pass");
    }

    public override void NodeCallback() {
        base.NodeCallback();

        if(eventId > -1)
            LoadedData.GetSingleton<Timer>().UpdateEventDuration(eventId, GetNodeVariable<float>("Duration"));
        else
            eventId = LoadedData.GetSingleton<Timer>().CreateNewTimerEvent(GetNodeVariable<float>("Duration"), this);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Duration",0)),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Value to Pass",0),VariableTypes.INTERCHANGEABLE)
        });
    }   
}




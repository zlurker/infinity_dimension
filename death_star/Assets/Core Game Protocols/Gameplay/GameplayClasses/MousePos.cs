﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();

        if(IsClientPlayerUpdate()) {
            Vector3 currPosInWorld = LoadedData.currSceneCamera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Send out mouse pos");
            SetVariable<Vector3>("Mouse Pos", currPosInWorld);
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Mouse Pos", new Vector3()), VariableTypes.NETWORK)
        });
    }
}

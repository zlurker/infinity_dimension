using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : AbilityTreeNode {

    public override void NodeCallback(int threadId) {

        if(IsClientPlayerUpdate()) {
            Vector2 currPosInWorld = LoadedData.currSceneCamera.ScreenToWorldPoint(Input.mousePosition);
            SetVariable<float[]>("Mouse Pos", new float[] { currPosInWorld.x, currPosInWorld.y });
        }
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<float[]>("Mouse Pos", null), VariableTypes.CLIENT_ACTIVATED)
        });
    }
}

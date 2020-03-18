using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : AbilityTreeNode {

    public const int MOUSE_POS = 0;

    public override void NodeCallback(int threadId) {

        if(IsClientPlayerUpdate()) {
            Vector2 currPosInWorld = LoadedData.currSceneCamera.ScreenToWorldPoint(Input.mousePosition);
            SyncDataWithNetwork<float[]>(MOUSE_POS, new float[] { currPosInWorld.x, currPosInWorld.y });
        }
    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<float[]>("Mouse Pos", null)
        };
    }
}

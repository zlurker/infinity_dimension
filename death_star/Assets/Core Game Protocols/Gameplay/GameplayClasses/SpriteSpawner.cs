using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpawner : AbilityTreeNode {

    public override void NodeCallback() {
        base.NodeCallback();
        SetVariable<Sprite>("Sprite", AbilitiesManager.aData[GetCentralInst().GetPlayerId()].assetData[GetNodeVariable<string>("Sprite Name")]);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[]{
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Sprite Name",""), VariableTypes.AUTO_MANAGED,VariableTypes.IMAGE_DEPENDENCY),
            new LoadedRuntimeParameters(new RuntimeParameters<Sprite>("Sprite",null))
        });
    }
}

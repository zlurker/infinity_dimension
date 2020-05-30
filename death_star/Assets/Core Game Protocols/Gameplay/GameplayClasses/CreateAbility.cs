using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : AbilityTreeNode, IOnVariableSet {

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        //Debug.Log("Construction phase called. LHS Links: " + data.GetLinkData(data.GetCurrBuildNode()).lHS.Count);
        data.AddTargettedNode(data.GetCurrBuildNode(), GetVariableId("Created Ability"), ON_VARIABLE_CATERGORY.ON_CHANGED, data.GetCurrBuildNode());
    }

    public void OnVariableSet(int varId) {
        if(varId == GetVariableId("Created Ability")) {
            //Debug.Log("info passed from callback");
            TriggerOnHostProcessed(GetNodeVariable<int[]>("Created Ability"));
        }
    }

    public override void NodeCallback() {
        base.NodeCallback();

        //Debug.Log("Node was callbacked!");
        //Debug.LogFormat("NodeID {0}, Thread ID {1}", name, GetNodeThreadId());

        if(IsHost()) {
            AbilityCentralThreadPool inst = GetCentralInst();
            AbilityCentralThreadPool newA = new AbilityCentralThreadPool(inst.GetPlayerId());
            //AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].SignalCentralCreation(newA);
            AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA, ClientProgram.clientId);
            newA.StartThreads(0);
            GetCentralInst().UpdateVariableValue<int[]>(GetNodeId(), GetVariableId("Created Ability"), new int[] { inst.GetPlayerId(), ClientProgram.clientId, newA.ReturnCentralId() }, true, false);
            HandlePostAbilityCreation();
            return;
        }

        //if(GetNodeVariable<int[]>("Created Ability") != null) 
            //TriggerOnHostProcessed(GetNodeVariable<int[]>("Created Ability"));      
    }

    void TriggerOnHostProcessed(int[] value) {
        AbilityCentralThreadPool newA = new AbilityCentralThreadPool(value[0]);
        AbilitiesManager.aData[value[0]].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA, value[1], value[2]);
        newA.StartThreads(0);
        HandlePostAbilityCreation();
    }

    void HandlePostAbilityCreation() {
        // Unsets input triggered.
        GetCentralInst().UpdateVariableValue<int[]>(GetNodeId(), GetVariableId("Created Ability"), null, false, false);

        //Debug.Log("Moving to Connected.");
        SetVariable<string>("Ability Name");
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {

        int aN = GetVariableId("Ability Name");

        if(variable == aN) {
            SpawnerOutput aNField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Dropdown>(aNField);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<string> rpS = rp as RuntimeParameters<string>;
            int selected = 0;

            foreach(var kPV in AbilityPageScript.abilityInfo) {
                dOd.Add(new Dropdown.OptionData(kPV.Value.n));

                if(kPV.Key == rpS.v)
                    selected = dOd.Count - 1;
            }

            dW.AddOptions(dOd);

            dW.value = selected;

            dW.onValueChanged.AddListener((id) => {
                string[] dirNames = AbilityPageScript.abilityInfo.Keys.ToArray();
                rpS.v = dirNames[id];
            });

            return aNField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Created Ability", null), VariableTypes.HIDDEN,VariableTypes.NETWORK),
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Ability Name", ""))         
        });
    }
}

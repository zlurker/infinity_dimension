using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : AbilityTreeNode, IOnVariableInterface {

    public override void ConstructionPhase(AbilityData data) {
        base.ConstructionPhase(data);

        //Debug.Log("Construction phase called. LHS Links: " + data.GetLinkData(data.GetCurrBuildNode()).lHS.Count);
        data.AddTargettedNode(data.GetCurrBuildNode(), GetVariableId("Created Ability"), ON_VARIABLE_CATERGORY.ON_CHANGED, data.GetCurrBuildNode());
    }

    public int CentralCallback<T>(T value, int nodeId, int varId, int links) {

        //SetVariable<bool>("Internal Input Track", (bool)(object)value);
        //Debug.Log("Central Callback!, " + value);

        //Debug.Log("Value of T" + value);
        //Debug.LogFormat("NodeID {0}, Thread ID {1}", name, GetNodeThreadId());

        if(GetNodeThreadId() > -1) {
            //Debug.Log("Has thread.");

            if((int[])(object)value != null) {
                //Debug.Log("Updated ability details.");
                TriggerOnHostProcessed((int[])(object)value);
            }
        }else
            GetCentralInst().UpdateVariableValue<T>(GetNodeId(), GetVariableId("Created Ability"), value, false, false);

        return 0;
    }

    public override void NodeCallback() {
        base.NodeCallback();

        //Debug.Log("Node was callbacked!");
        //Debug.LogFormat("NodeID {0}, Thread ID {1}", name, GetNodeThreadId());

        if(GetNodeVariable<int[]>("Created Ability") != null) {
            TriggerOnHostProcessed(GetNodeVariable<int[]>("Created Ability"));
        }

        if(ClientProgram.clientId == ClientProgram.hostId) {
            AbilityCentralThreadPool inst = GetCentralInst();

            AbilityCentralThreadPool newA = new AbilityCentralThreadPool(inst.GetPlayerId());
            //AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].SignalCentralCreation(newA);
            AbilitiesManager.aData[inst.GetPlayerId()].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA, ClientProgram.clientId);
            GetCentralInst().UpdateVariableValue<int[]>(GetNodeId(), GetVariableId("Created Ability"), new int[] { inst.GetPlayerId(), ClientProgram.clientId, newA.ReturnCentralId() },true,false);
            HandlePostAbilityCreation();
        }
    }

    void TriggerOnHostProcessed(int[] value) {
        AbilityCentralThreadPool newA = new AbilityCentralThreadPool(value[0]);
        AbilitiesManager.aData[value[0]].abilties[GetNodeVariable<string>("Ability Name")].CreateAbility(newA, value[1], value[2]);

        Debug.Log("Abilities created.");
        HandlePostAbilityCreation();
    }

    void HandlePostAbilityCreation() {
        // Unsets input triggered.
        GetCentralInst().UpdateVariableValue<int[]>(GetNodeId(), GetVariableId("Created Ability"), null, false, false);

        Debug.Log("Moving to Connected.");
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
            new LoadedRuntimeParameters(new RuntimeParameters<string>("Ability Name", "")),
            new LoadedRuntimeParameters(new RuntimeParameters<int[]>("Created Ability", null), VariableTypes.HIDDEN,VariableTypes.HOST_ACTIVATED)
        });
    }
}

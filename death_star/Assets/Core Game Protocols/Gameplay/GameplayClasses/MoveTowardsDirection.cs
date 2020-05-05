using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTowardsDirection : MoveTo {

    bool allDataRecv;
    Vector3 normDir;

    float timeDirChanged;
    Vector3 dirChangeStart;

    void Update() {

        if(allDataRecv) {
            if(GetNodeVariable<float>("Duration") > Time.realtimeSinceStartup - timeDirChanged) {
                float timeRatio = (Time.realtimeSinceStartup - timeDirChanged) / GetNodeVariable<float>("Duration");

                GetTargetTransform().position = dirChangeStart + (normDir * (GetNodeVariable<float>("Total Distance") * timeRatio));
            } else
                GetTargetTransform().position = dirChangeStart + (normDir * GetNodeVariable<float>("Total Distance"));

            SetVariable<Vector3>("Current Target Position", GetTargetTransform().position);
        }

    }

    public override void NodeCallback() {
        overrode = true;
        base.NodeCallback();

        Debug.Log("Movement called.");

        allDataRecv = CheckIfVarRegionBlocked("Coordinates", "Target", "Total Distance", "Duration");

        if(allDataRecv) {
            Vector3 vToN = new Vector3();

            switch(GetNodeVariable<int>("Coordinate Type")) {

                // Direction
                case 0:
                    vToN = GetNodeVariable<Vector3>("Coordinates");
                    break;
                
                // Actual
                case 1:
                    vToN = GetNodeVariable<Vector3>("Coordinates") - GetTargetTransform().transform.position;
                    //Debug.Log(vToN);
                    break;
            }

            normDir = vToN.normalized;
            normDir.z = 0;

            dirChangeStart = GetTargetTransform().position;
            timeDirChanged = Time.realtimeSinceStartup;
        }
    }

    public override SpawnerOutput ReturnCustomUI(int variable, RuntimeParameters rp) {
       
        int o = GetVariableId("Coordinate Type");

        if(o == variable) {
            SpawnerOutput oField = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper));
            Dropdown dW = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Dropdown>(oField);
            List<Dropdown.OptionData> dOd = new List<Dropdown.OptionData>();
            RuntimeParameters<int> rpI = rp as RuntimeParameters<int>;

            dOd.Add(new Dropdown.OptionData("Direction"));
            dOd.Add(new Dropdown.OptionData("Actual Position"));

            dW.AddOptions(dOd);

            dW.value = rpI.v;

            dW.onValueChanged.AddListener((id) => {
                rpI.v = id;
            });

            return oField;
        }

        return base.ReturnCustomUI(variable, rp);
    }

    public override void GetRuntimeParameters(List<LoadedRuntimeParameters> holder) {
        base.GetRuntimeParameters(holder);

        holder.AddRange(new LoadedRuntimeParameters[] {
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Total Distance",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<float>("Duration",1),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<int>("Coordinate Type",0),VariableTypes.AUTO_MANAGED),
            new LoadedRuntimeParameters(new RuntimeParameters<Vector3>("Current Target Position",new Vector3()))
        });
    }
}

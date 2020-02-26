using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest : AbilityTreeNode {


    float initialTimer;
    bool timerReset = true;
    bool reset;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        //Debug.LogFormat("Current timer {0}/{1}, bool {2}", Time.time, initialTimer,timerReset);

        if(initialTimer < Time.time && !timerReset) {
            Debug.Log("Condition fulfilled, threadID:" + GetNodeThreadId());
            //Debug.Log("Current node thread:" + GetNodeThreadId());
            //Debug.LogFormat("curr node {0}, nodeValue{1}", GetNodeId(), TravelThread.globalCentralList.l[GetCentralId()].ReturnRuntimeParameter<int>(GetNodeId(), 0).v);
            timerReset = true;
          
            TravelThread.globalCentralList.l[GetCentralId()].NodeVariableCallback<string>(GetNodeThreadId(), 0, "0");
            //Debug.Log("Current node thread:" + GetNodeThreadId());            
        }

        if(reset) {
            initialTimer = Time.time + 2;
            timerReset = false;
            reset = false;
        }

    }

    public override RuntimeParameters[] GetRuntimeParameters() {
        return new RuntimeParameters[] {
            new RuntimeParameters<string>("Testing","")
        };
    }

    public override void NodeCallback(int threadId) {
        Debug.Log("Timer reset.");
        reset = true;       
    }

}

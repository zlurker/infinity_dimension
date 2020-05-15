using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimerCallback {
    void CallOnTimerEnd(int eventId);
}

public class TimerData {

    public float startTime;
    public float duration;
    public ITimerCallback callback;

    public TimerData(float sT, float d, ITimerCallback cb) {
        startTime = sT;
        duration = d;
        callback = cb;
    }
}

public class Timer : MonoBehaviour, ISingleton {

    Dictionary<int, TimerData> tData;
    int currTimerCreated;

    void Update() {

        List<int> delTimers = new List<int>();

        // Need to modify this to only accept those with 
        foreach(var timeData in tData) {
            Debug.Log("Curr Timer ID Check: " + timeData.Key);

            if(timeData.Value.startTime + timeData.Value.duration <= Time.realtimeSinceStartup) {
                
                timeData.Value.callback.CallOnTimerEnd(timeData.Key);
                delTimers.Add(timeData.Key);
                //tData.Remove(timeData.Key);
            }
        }

        for(int i = 0; i < delTimers.Count; i++)
            tData.Remove(delTimers[i]);

    }

    public int CreateNewTimerEvent(float d, ITimerCallback cb) {

        tData.Add(currTimerCreated, new TimerData(Time.realtimeSinceStartup, d, cb));
        currTimerCreated++;
        return currTimerCreated - 1;
    }

    public int CreateNewTimerEvent(float sT, float d, ITimerCallback cb) {
        tData.Add(currTimerCreated, new TimerData(sT, d, cb));
        currTimerCreated++;
        return currTimerCreated -1;
    }

    public void UpdateEventStartTime(int eventId, float sT) {
        //Debug.LogFormat("Event {0}'s start time has been changed to {1}. End timing is now {2}", eventId, sT, sT + tData[eventId].duration);
        tData[eventId].startTime = sT;
    }

    public void UpdateEventDuration(int eventId, float d) {
        tData[eventId].duration = d;
    }

    public void RunOnCreated() {
        tData = new Dictionary<int, TimerData>();
        currTimerCreated = 0;
    }

    public void RunOnStart() {
    }
}

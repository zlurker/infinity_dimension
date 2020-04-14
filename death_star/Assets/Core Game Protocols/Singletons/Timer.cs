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

    EnhancedList<TimerData> tData;

    void Update() {

        for(int i = tData.l.Count-1; i >= 0; i--) {
            if(tData.l[i] != null)
                if(tData.l[i].startTime + tData.l[i].duration <= Time.realtimeSinceStartup) {
                    tData.l[i].callback.CallOnTimerEnd(i);
                    tData.Remove(i);
                }
        }
    }

    public int CreateNewTimerEvent(float d, ITimerCallback cb) {
        return tData.Add(new TimerData(Time.realtimeSinceStartup, d, cb));
    }

    public int CreateNewTimerEvent(float sT, float d, ITimerCallback cb) {
        return tData.Add(new TimerData(sT, d, cb));
    }

    public void UpdateEventStartTime(int eventId, float sT) {
        Debug.LogFormat("Event {0}'s start time has been changed to {1}. End timing is now {2}",eventId,sT,sT + tData.l[eventId].duration);
        tData.l[eventId].startTime = sT;
    }

    public void UpdateEventDuration(int eventId, float d) {
        tData.l[eventId].duration = d;
    }

    public void RunOnCreated() {
        tData = new EnhancedList<TimerData>();
    }

    public void RunOnStart() {
    }
}

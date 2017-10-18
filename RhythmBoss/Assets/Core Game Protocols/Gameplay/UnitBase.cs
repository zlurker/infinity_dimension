using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public struct EffectTimerChecker {
    public float startTime;
    public int tickCount;

    public EffectTimerChecker(float sT, int tC) {
        startTime = sT;
        tickCount = tC;
    }
}

public struct CurrentEffects {
    public EffectTemplate effect;
    public EffectTimerChecker time;
}

public class UnitBase : MonoBehaviour {

    public Stat[] stat;
    public List<CurrentEffects> currEffects;

    void Start() {
        stat = (Stat[]) GlobalData.sT.Clone();
        currEffects = new List<CurrentEffects>();

        AddEffect(new EffectTemplate("Max ", 10, 0.0001f, "", 10, false,0));
    }

    void Update() {
        #region Effects Checking
        for (int i = 0; i < currEffects.Count; i++) {
            float time = Time.time > currEffects[i].time.startTime + currEffects[i].effect.duration ? currEffects[i].time.startTime + currEffects[i].effect.duration : Time.time; //Adjusts time to make sure it doesn't exceed
            int currTick = Mathf.FloorToInt((time - currEffects[i].time.startTime + currEffects[i].effect.tickCount) / currEffects[i].effect.tickCount); //Calculates tick past since last frame

            if (currTick != currEffects[i].time.tickCount) {
                float currEffV = currEffects[i].effect.value * (currTick - currEffects[i].time.tickCount);
                stat[GlobalData.IterateKey(stat, currEffects[i].effect.statAffected)].v[currEffects[i].effect.vM] -= currEffV; //Affects value according to ticks past

                CurrentEffects instance = currEffects[i];
                instance.time.tickCount = currTick;
                currEffects[i] = instance; //Updates tick to return to original stat

                Debug.Log(currEffects[i].effect.statAffected + " left: " + stat[GlobalData.IterateKey(stat, currEffects[i].effect.statAffected)].v[currEffects[i].effect.vM] + " Current Tick: " + currTick);
            }

            if (currEffects[i].time.startTime + currEffects[i].effect.duration <= Time.time) { //Checks if duration is over
                if (!currEffects[i].effect.permanent) //If not permanent, returns all values lost though this effect to stat
                    stat[GlobalData.IterateKey(stat, currEffects[i].effect.statAffected)].v[currEffects[i].effect.vM] += currEffects[i].time.tickCount * currEffects[i].effect.value;

                Debug.Log(currEffects[i].effect.statAffected + " left: " + stat[GlobalData.IterateKey(stat, currEffects[i].effect.statAffected)].v[currEffects[i].effect.vM]);
                currEffects.RemoveAt(i); //Removes effect
            }
        }
        #endregion
    }

    public void AddEffect(EffectTemplate effect) {
        CurrentEffects instance = new CurrentEffects(); 
        instance.effect = effect;
        instance.time = new EffectTimerChecker(Time.time, 0); //Calibrates timing according to time effect started

        currEffects.Add(instance);
    }
}

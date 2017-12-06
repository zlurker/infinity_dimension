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

    void Awake() {
        stat = (Stat[]) PresetGameplayData.sT.Clone();
        currEffects = new List<CurrentEffects>();

        AddEffect(new EffectTemplate("Max Health", 10, 0.0001f, "", 10, false));
    }

    void Update() {
        #region Effects Checking
        for (int i = 0; i < currEffects.Count; i++) {
            float time = Time.time > currEffects[i].time.startTime + currEffects[i].effect.duration ? currEffects[i].time.startTime + currEffects[i].effect.duration : Time.time; //Adjusts time to make sure it doesn't exceed
            int currTick = Mathf.FloorToInt((time - currEffects[i].time.startTime + currEffects[i].effect.tickCount) / currEffects[i].effect.tickCount); //Calculates tick past since last frame

            if (currTick != currEffects[i].time.tickCount) {
                float currEffV = currEffects[i].effect.value * (currTick - currEffects[i].time.tickCount);
                stat[BaseIteratorFunctions.IterateKey(stat, currEffects[i].effect.statAffected)].v-= currEffV; //Affects value according to ticks past

                CurrentEffects instance = currEffects[i];
                instance.time.tickCount = currTick;
                currEffects[i] = instance; //Updates tick to return to original stat

                Debug.Log(currEffects[i].effect.statAffected + " left: " + stat[BaseIteratorFunctions.IterateKey(stat, currEffects[i].effect.statAffected)].v + " Current Tick: " + currTick);
            }

            if (currEffects[i].time.startTime + currEffects[i].effect.duration <= Time.time) { //Checks if duration is over
                if (!currEffects[i].effect.permanent) //If not permanent, returns all values lost though this effect to stat (Regains how much it was lost in the time it was struck in the effect.
                    stat[BaseIteratorFunctions.IterateKey(stat, currEffects[i].effect.statAffected)].v += currEffects[i].time.tickCount * currEffects[i].effect.value;

                Debug.Log(currEffects[i].effect.statAffected + " left: " + stat[BaseIteratorFunctions.IterateKey(stat, currEffects[i].effect.statAffected)].v);
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

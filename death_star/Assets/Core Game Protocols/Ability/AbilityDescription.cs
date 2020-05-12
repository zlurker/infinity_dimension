using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartNodeType {
    THREAD_SPLITTER, REPEATER
}

public class AbilityInfo {
    public string n;
    public string d;
    public int kC;
    public int sNT;
    public float sNA;

    public AbilityInfo() {
        n = "New Ability";
        d = "No Description";
        kC = 0;
        sNT = (int)StartNodeType.THREAD_SPLITTER;
        sNA = 1;
    }
}

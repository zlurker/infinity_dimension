using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region Gameplay Data Structures
public enum RhythmAnalyseState {
    Manual, Auto
}

public struct EffectTemplate {
    public string statAffected;
    public float duration;
    public float tickCount;
    public string operation;
    public float value;
    public bool permanent;

    public EffectTemplate(string sA, float d, float tC, string o, float v, bool p) {
        statAffected = sA;
        duration = d;
        tickCount = tC;
        operation = o;
        value = v;
        permanent = p;
    }
}

public struct JudgementRange {
    public string name;
    public float maxWindow;
    public int counter;

    public JudgementRange(string n, float mW, int c) {
        name = n;
        maxWindow = mW;
        counter = c;
    }
}

public class Stat : BaseIterator {
    public float v;
    public float pTC;

    public Stat(string name, float val) {
        n = name;
        v = val;
    }
}

[System.Serializable]
public class UIElement : BaseIterator {
    public MaskableGraphic u;
    public float dAT;

    public UIElement(string name, MaskableGraphic ui, float deActivateTiming) {
        n = name;
        u = ui;
    }
}

public class InputData:BaseIterator {
    public List<r> rs;
    public KeyCode k;
    public int b;

    public InputData(KeyCode key, string id) {
        n = id;
        k = key;
        rs = new List<r>();
    }

    public InputData(int button, string id) {
        n = id;
        b = button;
        rs = new List<r>();
    }
}

public class TimeData : BaseIterator {
    public List<r> tD; //timeDelegates
    public float eT; //endTime

    public TimeData(string id,float endTime) {
        n = id;
        eT = endTime;
        tD = new List<r>();
    }
}

public struct PointData {
    public float aC; //angleChanges
    public float u; //unit

    public PointData(float angleChanges, float unit) {
        aC = angleChanges;
        u = unit;
    }
}
#endregion

#region General Data Structures
public delegate void r();
public delegate void jD(int jG);

[System.Serializable]
public class BaseIterator {
    public string n;
}
#endregion

public static class GlobalData {
    #region Gameplay Global Data
    public static float bpm;
    public static float offset;
    public static bool followsBeat = false;
    public static RhythmAnalyseState analyseAlgorithm;

    public static AudioClip song;
    #endregion
    
    //Loads a new level and refreshes data structures if needed.
    public static void LoadNewLevel(int level) {
        PlayerInput.i.iS = new List<InputData>();
        DelegatePools.ClearDelegatePools();
        SceneManager.LoadScene(level);
    }
}

public static class DelegatePools {
    public static List<jD> jD;

    public static void ClearDelegatePools() {
        jD = new List<jD>();
    }
}

public static class BaseIteratorFunctions { //A list of functions that complements the BaseIterator class
   
    //Iterates though the Array and returns the first item with the string k
    public static int IterateKey(BaseIterator[] tA, string k) {
        for (int i = 0; i < tA.Length; i++)
            if (tA[i].n == k)
                return i;

        Debug.LogErrorFormat("The key: {0} does not exist.", k);
        return -1;
    }
}

public static class PresetGameplayData {
    public static Stat[] sT = new Stat[]
  { new Stat("Current Health", 50), new Stat("Max Health", 50), new Stat("Movespeed", 1), new Stat("Health Regeneration", 1)  };

    public static JudgementRange[] jRT = new JudgementRange[]
    { new JudgementRange("YEAH!", 0.05f, 0), new JudgementRange("SUPER", 0.1f, 0), new JudgementRange("GOOD", 0.2f, 0), new JudgementRange("OK", 0.3f, 0) };
}

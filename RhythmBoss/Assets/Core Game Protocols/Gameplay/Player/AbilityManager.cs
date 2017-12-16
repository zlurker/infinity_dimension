using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire {
    public IPlayerEditable mTR; //methodsToRun
    public object[] p; //parameters

    public Fire(IPlayerEditable methodsToRun, object[] parameters) {
        mTR = methodsToRun;
        p = parameters;
    }
}

public class Ability : BaseIterator {
    public Fire[] aF; //abilityFires

    public Ability(Fire[] abilityFire) {
        aF = abilityFire;
    }

    public Ability(string name, Fire[] abilityFire) {
        n = name;
        aF = abilityFire;
    }
}


public class AbilityManager : MonoBehaviour {

    Ability[] a; //abilities
    int aS; //abilitySelected

    void Start() {
        DelegatePools.jD.Add(FireAbility);

        a = new Ability[] { new Ability(new Fire[] {
            new Fire(GameObjectSpawner.i, new object[] {"Test",1 })
        })};
    }

    void Update() {

    }

    void FireAbility(int g) {
        //Debug.Log(PresetGameplayData.jRT[g].name);

        for (int i = 0; i < a[aS].aF.Length; i++) 
            a[aS].aF[i].mTR.Fire(a[aS].aF[i].p);        
    }
}

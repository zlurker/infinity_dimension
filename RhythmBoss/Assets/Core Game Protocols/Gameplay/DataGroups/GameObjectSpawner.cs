using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : BaseIterator {
    public Pool<MonoBehaviour> gOP;

    public GameObjectPool(string name, CI instanceCreator) {
        n = name;
        gOP = new Pool<MonoBehaviour>(instanceCreator, name);
    }
}

[System.Serializable]
public class SpawnInstance : BaseIterator {
    public MonoBehaviour i;
}

public class GameObjectSpawner : MonoBehaviour,IPlayerEditable {

    public static GameObjectSpawner i; //instance
    public SpawnInstance[] iS; //instanceS

    GameObjectPool[] gOP; //gameObjPool;
    public OnSpawn test;
    int cK; //currentKey

    void Start() {
        gOP = new GameObjectPool[iS.Length];

        for (int i = 0; i < gOP.Length; i++)
            gOP[i] = new GameObjectPool(iS[i].n, CreateNewObject);

        i = this;

        Spawn("Projectile", new Vector3());
    }

    public MonoBehaviour Spawn(string p, Vector3 l) {
        int cOK = BaseIteratorFunctions.IterateKey(gOP, p);
        cK = cOK;

        MonoBehaviour iR = gOP[cOK].gOP.Retrieve();
        iR.gameObject.SetActive(true);
        iR.transform.position = l;

        (iR as OnSpawn).RunOnActive();
        return iR;
    }

    public void Remove(MonoBehaviour iR, string p) {
        iR.gameObject.SetActive(false);
        gOP[BaseIteratorFunctions.IterateKey(gOP, p)].gOP.Store(iR);
    }

    Object CreateNewObject() {
        return Instantiate(iS[cK].i);
    }

    public void Fire(object[] parameters) {
        Debug.Log(parameters[0]);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner :Spawner,IPlayerEditable {

    public static GameObjectSpawner i; //instance

    // Use this for initialization
    void Start() {
        i = this;
        Spawn("Projectile",new Vector3());
    }

    // Update is called once per frame
    void Update() {

    }

    public void Fire(object[] parameters) {

    }

    public void LoadUI() {

    }
}

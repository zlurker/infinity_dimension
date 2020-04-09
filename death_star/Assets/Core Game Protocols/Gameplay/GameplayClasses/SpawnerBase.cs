using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBase : AbilityTreeNode, IOnSpawn {

    protected SpriteRenderer sR;
    protected Rigidbody2D rB;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSpawn() {
        if(sR == null)
            sR = GetComponent<SpriteRenderer>();

        if(rB == null) {
            rB = GetComponent<Rigidbody2D>();
            rB.gravityScale = 0;
            rB.mass = 0;
            rB.drag = 0;
            rB.angularDrag = 0;
            rB.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}

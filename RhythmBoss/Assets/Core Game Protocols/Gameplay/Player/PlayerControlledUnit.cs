using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledUnit : UnitBase {

    // Use this for initialization
    void Start() {
        PlayerInput.i.AddNewInput(KeyCode.W, Up);
        PlayerInput.i.AddNewInput(KeyCode.A, Down);
        PlayerInput.i.AddNewInput(KeyCode.D, Right);
        PlayerInput.i.AddNewInput(KeyCode.S, Left);        
    }

    // Update is called once per frame
    void Update() {

    }

    void UpdateLocation(Vector3 v) {
        transform.position += v;
    }

    void Up() {
        UpdateLocation(new Vector3(0,stat[BaseIteratorFunctions.IterateKey(stat,"Movespeed")].v));
    }

    void Down() {
        UpdateLocation(new Vector3(0, -stat[BaseIteratorFunctions.IterateKey(stat, "Movespeed")].v));
    }
    void Right() {
        UpdateLocation(new Vector3(stat[BaseIteratorFunctions.IterateKey(stat, "Movespeed")].v, 0));
    }

    void Left() {
        UpdateLocation(new Vector3(-stat[BaseIteratorFunctions.IterateKey(stat, "Movespeed")].v, 0));
    }
}

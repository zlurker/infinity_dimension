using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledUnit : UnitBase {

    // Use this for initialization
    void Start() {
        PlayerInput.i.AddNewInput(KeyCode.W, new DH(Up),1);
        PlayerInput.i.AddNewInput(KeyCode.A, new DH(Down),1);
        PlayerInput.i.AddNewInput(KeyCode.D, new DH(Right),1);
        PlayerInput.i.AddNewInput(KeyCode.S, new DH(Left),1);        
    }

    // Update is called once per frame
    void Update() {

    }

    void UpdateLocation(Vector3 v) {
        transform.position += v;
    }

    void Up(object[] p) {
        UpdateLocation(new Vector3(0,Iterator.ReturnObject<Stat>(stat,"Movespeed").v));
    }

    void Down(object[] p) {
        UpdateLocation(new Vector3(0, -Iterator.ReturnObject<Stat>(stat, "Movespeed").v));
    }
    void Right(object[] p) {
        UpdateLocation(new Vector3(Iterator.ReturnObject<Stat>(stat, "Movespeed").v, 0));
    }

    void Left(object[] p) {
        UpdateLocation(new Vector3(-Iterator.ReturnObject<Stat>(stat, "Movespeed").v, 0));
    }
}

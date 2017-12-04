using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Vector2[] pTC;

    void Start() {
        Debug.Log(Math.Normalise(new Vector2(-1,0)));
        Debug.Log(Math.CalculateAngle(pTC));
    }   
}

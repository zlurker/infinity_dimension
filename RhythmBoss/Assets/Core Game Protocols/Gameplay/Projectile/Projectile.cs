using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Vector2[] pTC;

    void Start() {

        Debug.Log(Math.CalculateAngle(pTC));
    }

    
}

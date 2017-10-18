using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : UIStorer {

    // Use this for initialization
    public float angle;
    float radius;

    // Update is called once per frame
    void Update () {
        radius += 0.1f;
        Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        //radius += 1f;

        transform.position = pos;
    }
}

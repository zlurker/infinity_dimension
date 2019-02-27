using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math {

    public static float CalculateAngle(Vector2 e) {
        float total = Mathf.Abs(e.x) + Mathf.Abs(e.y);
        float ratio = 1 - (e.y / total);
        float startUp = 0;

        if(e.x >= 0) {
            startUp = 180;
            ratio = (e.y / total) - (-1);
        }

        return (ratio / 2 * 180) + startUp;
    }

    public static Vector2 Normalise(Vector2 tTN) {
        float sF = Mathf.Abs(tTN.x) > Mathf.Abs(tTN.y) ? tTN.x : tTN.y;
        Debug.Log(tTN);

        for (int j = 0; j < 2; j++)
            if (tTN[j] != 0)
                tTN[j] /= Mathf.Abs(sF);

        Debug.Log(tTN);
        return tTN;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math {

    public static float CalculateAngle(Vector2 e) {
        float total = Mathf.Abs(e.x) + Mathf.Abs(e.y);
        float ratio = -1;
        float startUp = 0;

        if(e.x >= 0) {
            startUp = 180;
            ratio = 1;
        }

        ratio = ratio * (e.y / total) + 1;
        startUp += (ratio / 2 * 180);

        if(startUp != startUp)
            startUp = 0;

        return startUp;
    }

    public static Vector2 Normalise(Vector2 tTN) {
        float sF = Mathf.Abs(tTN.x) > Mathf.Abs(tTN.y) ? tTN.x : tTN.y;

        for(int j = 0; j < 2; j++)
            if(tTN[j] != 0)
                tTN[j] /= Mathf.Abs(sF);

        return tTN;
    }
}

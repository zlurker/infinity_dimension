using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math {

    public static float CalculateAngle(Vector2[] pTC) {
        for (int i = 0; i < pTC.Length; i++)
            pTC[i] = Normalise(pTC[i]);

        Vector2 d = pTC[1] - pTC[0];
        int m = 0;
        int u = 45;

        for (int i = 0; i < 2; i++)
            if (pTC[0][i] == 0 && pTC[1][i] == 0)
                m += 2;

        u = (pTC[0].y * d.x) + (pTC[0].x * d.y * -1) >= 0 ? u : u * -1;

        return (Mathf.Abs(d.x) + Mathf.Abs(d.y) + m) * u;
    }

    public static Vector2 Normalise(Vector2 tTN) {
        int i = Mathf.Abs(tTN.x) > Mathf.Abs(tTN.y) ? 0 : 1;

        for (int j = 0; j < 2; j++)
            if (tTN[j] != 0)
                tTN[j] /= Mathf.Abs(tTN[i]);

        return tTN;
    }
}

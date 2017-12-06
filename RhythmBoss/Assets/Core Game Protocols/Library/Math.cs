using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math {
    static Vector2[] cP = new Vector2[] { new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1) };

    public static float CalculateAngle(Vector2 s, Vector2 e) {
        s = Normalise(s);
        e = Normalise(e);

        Vector2 d = e - s;
        int m = 0;
        int u = 45;

        for (int i = 0; i < 2; i++)
            if (s[i] == 0 && e[i] == 0)
                m += 2;

        u = (s.y * d.x) + (s.x * d.y * -1) >= 0 ? u : u * -1;

        return (Mathf.Abs(d.x) + Mathf.Abs(d.y) + m) * u;
    }

    public static Vector2 Normalise(Vector2 tTN) {
        int i = Mathf.Abs(tTN.x) > Mathf.Abs(tTN.y) ? 0 : 1;

        for (int j = 0; j < 2; j++)
            if (tTN[j] != 0)
                tTN[j] /= Mathf.Abs(tTN[i]);

        return tTN;
    }

    public static float CheckIfAngleInRange(float a) {
        a = a % 360;

        if (a < 0)
            return 360 - a;

        return a;
    }

    public static Vector3 VectorFromAngle(float a) {
        a = CheckIfAngleInRange(a);

        int sI = Mathf.FloorToInt(a / 90);
        Vector2 aO = (cP[sI + 1] - cP[sI]) * ((a / 90) - sI);

        return cP[sI] + aO;
    }
}

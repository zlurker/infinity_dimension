using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternControl : MonoBehaviour {

    public static void Pattern_Args(string patterns, PoolElement[] objects, object[][] arg_values) {

        string[] pattern = patterns.Split(',');

        for (int i = 0; i < pattern.Length; i++)
            switch (pattern[i]) {

                case "VECTOR_PATTERN":
                    Vector3 s_P = (Vector3)arg_values[i][0];
                    Vector3 v_P = (Vector3)arg_values[i][1];

                    for (int j = 0; j < objects.Length; j++)
                        objects[j].o.transform.position = s_P + (v_P * j);
                    break;

                case "GROUP_PATTERN":

                    break;
            }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;

public struct LineData {
    public Transform s;
    public Transform e;
    public SpawnerOutput l;

    public LineData(Transform start, Transform end, SpawnerOutput line) {
        s = start;
        e = end;
        l = line;        
    }
}
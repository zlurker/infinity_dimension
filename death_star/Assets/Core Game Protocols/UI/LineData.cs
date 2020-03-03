﻿using UnityEngine;
using UnityEngine.UI;
using System;

public struct LineData {
    public Transform s;
    public Transform e;
    public SpawnerOutput line;

    public LineData(Transform start, Transform end) {
        s = start;
        e = end;
        line = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image));
        UIDrawer.GetTypeInElement<Image>(line).rectTransform.pivot = new Vector2(0.5f,0);
    }
}
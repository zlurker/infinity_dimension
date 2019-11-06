using UnityEngine;
using UnityEngine.UI;
using System;

public struct LineData {
    public Transform s;
    public Transform e;
    public ScriptableObject line;

    public LineData(Transform start, Transform end) {
        s = start;
        e = end;
        line = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Image) });
        Spawner.GetCType<Image>(line).rectTransform.pivot = new Vector2(0.5f,0);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public class LineManager:IWindowsDragEvent {

    public List<LineData> lineData;

	public LineManager() {
        lineData = new List<LineData>();
    }

    public void OnDrag() {
        UpdateLines();
    }
 

    public void UpdateLines() {
        for (int i =0; i < lineData.Count;i++) {
            lineData[i].line.transform.position = lineData[i].s.position;


            Vector2 d = lineData[i].e.position - lineData[i].s.position;
            Spawner.GetCType<Image>(lineData[i].line).rectTransform.sizeDelta = new Vector2(10f, d.magnitude);
            lineData[i].line.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(d)));
            Debug.Log("Working");
        }
        
    }

}

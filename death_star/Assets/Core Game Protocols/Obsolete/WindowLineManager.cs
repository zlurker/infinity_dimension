using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WindowLineManager {

    public List<LineData> lineData;

	public WindowLineManager() {
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
            //Debug.Log("Working");
        }
        
    }

}

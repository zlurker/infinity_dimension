using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MaskableGraphic {

    Vector3[] coordinates;
    Image test;

     void Start()
    {
        
        
        
        coordinates = new Vector3[] { new Vector3(-100, -100, 0), new Vector3(-100, 100, 0), new Vector3(100, 100, 0), new Vector3(100, -100, 0) };
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //vh.AddVert(new Vector3(-100, -100, 0), Color.black, Vector2.zero);
        //vh.AddVert(new Vector3(-100, -100, 0), Color.white, Vector2.up);
        //vh.AddVert(new Vector3(100, 100, 0), Color.white, Vector2.one);
        //vh.AddVert(new Vector3(100, -100, 0), Color.white, Vector2.right);

        //vh.AddVert(new Vector3(-100, 100, 0), Color.blue, Vector2.zero);
        //vh.AddVert(new Vector3(-100, 300, 0), Color.blue, Vector2.up);
        //vh.AddVert(new Vector3(100, 300, 0), Color.blue, Vector2.one);
        // vh.AddVert(new Vector3(100, 100, 0), Color.blue, Vector2.right);

        for (int i = 0; i < coordinates.Length; i++)
            vh.AddVert(coordinates[i],Color.black, Vector2.up);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);

        //vh.AddTriangle(4, 5, 6);
        //vh.AddTriangle(4, 6, 7);
    
    }

}

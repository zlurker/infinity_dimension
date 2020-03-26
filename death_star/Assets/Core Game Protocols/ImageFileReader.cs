using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageFileReader : MonoBehaviour {

    public SpriteRenderer display;
    Texture2D tex = null;

    public static Color[,] ReadImageFile(Color[] pixels) {
        
        int rcs = 16;
        int PNGDimensions = (int)Mathf.Sqrt(pixels.Length);
        int pngScaleFactor = PNGDimensions / rcs;
        Color[,] returnedData = new Color[rcs, rcs];

        for(int i = 0; i < rcs; i++) {
            int xStartPos = pngScaleFactor * i;
            for(int j = 0; j < rcs; j++) {
                int yStartPos = pngScaleFactor * j;
                returnedData[i, j] = pixels[(yStartPos * PNGDimensions) + xStartPos];
            }
        }

        return returnedData;
    }
}
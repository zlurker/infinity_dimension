using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PixelArtExperiment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler
{

    Texture2D colorTest;
    Color[] colors;

    bool inObject;
    Camera cam;
    public int rcs;
    public int PNGDimensions;
    public float dimensions;

    float scaleFactor;
    ScriptableObject pointer;
    public ScriptableObject[,] imageData;

    Vector2 pixelOffset;
    Vector2 lw;
    Vector2 mPos;



    // Use this for initialization
    void Start()
    {
        mPos = new Vector2();
        rcs = 16;
        dimensions = 450;
        PNGDimensions = 1024;
        scaleFactor = dimensions / rcs;

        imageData = new ScriptableObject[rcs, rcs];

        Vector2 imageDimensions = new Vector2(dimensions, dimensions);
        (transform as RectTransform).sizeDelta = imageDimensions;
        transform.localPosition = -(imageDimensions / 2);

        lw = new Vector2(dimensions / rcs, dimensions / rcs);

        pixelOffset = lw / 2;

        ScriptableObject sO = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });
        UIDrawer.GetCType<Button>(sO).onClick.AddListener(SavePNG);

        CreateNewPixel();

        /*colors = new Color[1000];
        for (int i = 0; i < colors.Length; i++)
            if (i < 500)
                colors[i] = Color.red;
            else
                colors[i] = Color.black;

        colorTest = new Texture2D(100, 10);
        colorTest.SetPixels(colors);
        
        File.WriteAllBytes(Application.dataPath + "/../test6.png", colorTest.EncodeToPNG());*/
    }

    // Update is called once per frame
    void Update()
    {
        if (inObject)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, cam, out mPos);

            for (int i = 0; i < 2; i++)
                mPos[i] = mPos[i] - (mPos[i] % scaleFactor);

            pointer.transform.localPosition = mPos + pixelOffset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!cam)
            cam = eventData.pressEventCamera;

        Debug.Log(Input.mousePosition);
        Debug.Log(eventData.position);

        inObject = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inObject = false;
    }

    void CreateNewPixel()
    {
        if (imageData[Mathf.RoundToInt(mPos.x / scaleFactor), Mathf.RoundToInt(mPos.y / scaleFactor)] == null)
        {
            imageData[Mathf.RoundToInt(mPos.x / scaleFactor), Mathf.RoundToInt(mPos.y / scaleFactor)] = pointer;
            ScriptableObject inst = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] {typeof(Image) });
            (Spawner.GetCType<Image>(inst).transform as RectTransform).sizeDelta = lw;
            inst.transform.parent = transform;
            pointer = inst;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CreateNewPixel();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        CreateNewPixel();
    }

    public void SavePNG()
    {
        colors = new Color[PNGDimensions * PNGDimensions];
        int pngScaleFactor = PNGDimensions / rcs;
        int totalValueModified = 0;

        for (int i = 0; i < rcs; i++)
        {
            int xStartPos = pngScaleFactor * i;
            for (int j = 0; j < rcs; j++)
            {
                int yStartPos = pngScaleFactor * j;

                if (imageData[i, j] != null)         
                    for (int k = xStartPos; k < pngScaleFactor+xStartPos; k++)
                        for (int l = yStartPos; l < pngScaleFactor+yStartPos; l++)
                        {
                            colors[(PNGDimensions * l) + k] = Color.black;
                            totalValueModified++;
                        }               
            }
        }

        colorTest = new Texture2D(PNGDimensions,PNGDimensions);
        colorTest.SetPixels(colors);

        Debug.LogFormat("Pixels modified. Total modified: {0}",pngScaleFactor);
        File.WriteAllBytes(Application.dataPath + "/../PixelCharacter.png", colorTest.EncodeToPNG());
    }
}

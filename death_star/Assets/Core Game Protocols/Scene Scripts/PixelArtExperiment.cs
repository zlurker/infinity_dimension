using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PixelArtExperiment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler {

    Texture2D colorTest;
    Color[] colors;

    bool inObject;
    Camera cam;
    public int rcs;
    public int PNGDimensions;
    public float dimensions;

    float scaleFactor;
    SpawnerOutput pointer;
    public SpawnerOutput[,] imageData;

    Vector2 pixelOffset;
    Vector2 lw;
    Vector2 mPos;

    Color[,] colorData;

    string name;

    void Start() {
        mPos = new Vector2();
        rcs = 16;
        dimensions = 450;
        PNGDimensions = 1024;

        CalibrateEditor();

        SpawnerOutput sO = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(sO).onClick.AddListener(SavePNG);
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(sO,"Text").text = "Save Art";

        SpawnerOutput nO = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(InputFieldWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(nO).onValueChanged.AddListener((s) => {
            name = s;
        });

        sO.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));
        nO.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.8f));

        GeneratePixels();
        pointer = CreatePixel();

        
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

    void GeneratePixels() {
        byte[] fileData;
        string path;
        Texture2D tex;
        colorData = new Color[100,10];

        path = Path.Combine(new string[] { LoadedData.gameDataPath, "Datafiles", "2", "ImageAssets", "Bullet" });
        path += ".png";

        if(File.Exists(path)) {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData);
            //ImageConversion.LoadImage(tex, fileData);

            //display.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2());
            colorData = ImageFileReader.ReadImageFile(tex.GetPixels());
        }

        for (int i=0; i < colorData.GetLength(0); i++) 
            for (int j =0; j < colorData.GetLength(1); j++) 
                if (colorData[i,j].a > 0) {
                    SpawnerOutput inst = CreatePixel();
                    LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(inst).color = colorData[i, j];
                    inst.script.transform.localPosition = new Vector2(scaleFactor *i,scaleFactor*j) + pixelOffset;
                    imageData[i, j] = inst;
                }          
            
        
    }

    void CalibrateEditor() {
        scaleFactor = dimensions / rcs;
        imageData = new SpawnerOutput[rcs, rcs];
        Vector2 imageDimensions = new Vector2(dimensions, dimensions);
        (transform as RectTransform).sizeDelta = imageDimensions;
        transform.localPosition = -(imageDimensions / 2);

        lw = new Vector2(dimensions / rcs, dimensions / rcs);

        pixelOffset = lw / 2;
    }

    // Update is called once per frame
    void Update() {
        if(inObject) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, cam, out mPos);

            for(int i = 0; i < 2; i++)
                mPos[i] = mPos[i] - (mPos[i] % scaleFactor);

            pointer.script.transform.localPosition = mPos + pixelOffset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(!cam)
            cam = eventData.pressEventCamera;

        //Debug.Log(Input.mousePosition);
        //sDebug.Log(eventData.position);

        inObject = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        inObject = false;
    }

    void CreateNewPixel() {
        if(imageData[Mathf.RoundToInt(mPos.x / scaleFactor), Mathf.RoundToInt(mPos.y / scaleFactor)] == null) {
            imageData[Mathf.RoundToInt(mPos.x / scaleFactor), Mathf.RoundToInt(mPos.y / scaleFactor)] = pointer;
            
            pointer = CreatePixel();
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        CreateNewPixel();
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("Dragging");
        CreateNewPixel();
    }

    SpawnerOutput CreatePixel() {
        SpawnerOutput inst = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(inst).rectTransform.sizeDelta = lw;
        inst.script.transform.parent = transform;

        return inst;
    }

    public void SavePNG() {
        colors = new Color[PNGDimensions * PNGDimensions];
        int pngScaleFactor = PNGDimensions / rcs;
        int totalValueModified = 0;

        for(int i = 0; i < rcs; i++) {
            int xStartPos = pngScaleFactor * i;
            for(int j = 0; j < rcs; j++) {
                int yStartPos = pngScaleFactor * j;

                for(int k = xStartPos; k < pngScaleFactor + xStartPos; k++)
                    for(int l = yStartPos; l < pngScaleFactor + yStartPos; l++) {
                        if(imageData[i, j] != null) {
                            colors[(PNGDimensions * l) + k] = Color.black;
                            totalValueModified++;
                        }                      
                    }
            }
        }

        colorTest = new Texture2D(PNGDimensions, PNGDimensions);
        colorTest.SetPixels(colors);

        string fP = FileSaver.PathGenerator(LoadedData.gameDataPath, new string[] { "UsrCreatedArt" });

        Debug.LogFormat("Pixels modified. Total modified: {0}", pngScaleFactor);
        File.WriteAllBytes(Path.Combine(fP, name + ".PNG"), colorTest.EncodeToPNG());
    }
}

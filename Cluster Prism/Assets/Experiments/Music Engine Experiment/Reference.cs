using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reference : MonoBehaviour {
    //[System.Serializable]
    //public struct PreviousData {
    // public float prevValue;
    // public bool upwards;
    // }
    public struct PointsData {
        public float max;
        public bool o;
    }

    public GameObject prefab;
    public GameObject cubesS;
    //public Bullet projectile;
    public int numberOfObjects;
    public int actualNumber = 20;
    public List<Renderer> cubes;
    public List<Renderer> cubesShadow;
    //public PreviousData[] prevData;
    public GameObject flash;
    public AudioSource song;
    public Text combo;
    public float minOnset;
    float flashValue;
    float[] max;
    int bpmTracker;
    int totalBeats;

    bool valueChanged;
    int currMinute;
    int selectedChannel;
    Vector3 peakHeight;
    Vector3 peakScale;

    float[] pV;
    float[] lV;
    bool[] cY;
    bool fA;

    //float lowestValue;
    //float gradient;
    //Renderer prevRenderer;

    void Start() {
        cubes = new List<Renderer>();
        cubesShadow = new List<Renderer>();

        for (int i = 0; i < actualNumber; i++) {
            //float angle = i * Mathf.PI * 2 / numberOfObjects;
            //Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 pos = new Vector3(i - (numberOfObjects / 2), 0, 0);
            cubes.Add(Instantiate(prefab, pos, Quaternion.identity).GetComponent<Renderer>());
            cubesShadow.Add(Instantiate(cubesS, pos, Quaternion.identity).GetComponent<Renderer>());
            //cubesShadow.Add(Instantiate(cubesS, pos, Quaternion.identity).GetComponent<Renderer>());
        }

        max = new float[actualNumber];
        pV = new float[actualNumber];
        lV = new float[actualNumber];
        cY = new bool[actualNumber];

        //prevData = new PreviousData[numberOfObjects];
        song = GetComponent<AudioSource>();
        /*float[] samples = new float[song.clip.samples * song.clip.channels];
        song.clip.GetData(samples, 0);
        int k = 0;
        int l = 0;
        Debug.Log(song.clip.samples + " " + song.clip.channels);
        //Debug.Log(song.clip.fr);
        while (l < 1000) {
            //samples[k] = samples[k] * 0.5F;
            if (samples[k] != 0) {
                ++l;

                Debug.Log(k + ": " + samples[k]);
            }
            k++;
        }*/

        //Debug.Log(l + "/" + samples.Length);
        //Debug.Log(samples.Length -l + "")
        //pD = new float[2];
        //pPD = new float[2];
    }

    // Update is called once per frame
    void Update() {
        float[] samples = new float[numberOfObjects];
        float currPeak = 0;
        float minVal = Mathf.Infinity;
        float highestValue = 0;
        AudioListener.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);


        /*if (prevData[selectedChannel].upwards)
            prevPeak = prevData[selectedChannel].prevValue;*/

        /*if (samples[selectedChannel] >= highestValue) {
            for (int i = 0; i < actualNumber; i++) {
                if (samples[i] > highestValue) {
                    highestValue = samples[i];
                    selectedChannel = i;
                }
            }
            fA = true;
        } else {
            Debug.Log("Working");
            if (fA) {
                flashValue = 2.5f;
                totalBeats++;
                combo.text = totalBeats.ToString();
                fA = false;
            }

            if (samples[selectedChannel] < highestValue * 0.1f)
                highestValue = samples[selectedChannel];
        }*/

        for (int i = 0; i < actualNumber; i++) {
            Vector3 previousScale = cubes[i].transform.localScale;
            previousScale.y = samples[i] * 100;
            cubes[i].transform.localScale = previousScale;

            Vector3 pos = cubes[i].transform.position;
            pos.y = cubes[i].transform.localScale.y / 2;
            cubes[i].transform.position = pos;


            //if (samples[i] > max[i]) {
                //max[i] = samples[i];
                
            //}


            /* if (currPeak < samples[i]) 
                 currPeak = samples[i];

              prevData[i].upwards = UpdatePrevData(i, samples[i]);
              prevData[i].prevValue = samples[i];


              if (prevData[i].prevValue > prevPeak) {
                  prevPeak = prevData[i].prevValue;
                  selectedChannel = i;

                  if (prevData[i].upwards) {
                      valueChanged = true;

                  }
              }

              if (prevData[i].upwards)
                  hasUpwards = true;*/
            /*if (samples[cP] >= pPV) { //Checks if current sample is going upwards or downwards. If upwards, then changing is allowed.
                if (samples[i] >= pPV) {
                    cP = i;
                    pPV = samples[i];
                }
            }*/

            if (samples[i] > highestValue) {
                highestValue = samples[i];
                selectedChannel = i;
            }
        }

        /*if (cP < 2)
            pD[0] = 0;
        else {
            float v = samples[cP] - samples[cP - 1];
            pD[0] = v / Mathf.Abs(v);
        }

        if (cP > actualNumber - 2)
            pD[1] = 0;
        else {
            float v = samples[cP + 1] - samples[cP];
            pD[1] = v / Mathf.Abs(v);
        }*/


        /*
        //Debug.Log(minVal);

        if (/*!prevData[selectedChannel].upwardscurrPeak - prevValue < 0 && currPeak - lowestValue >= minOnset) {
            //targetChanged = false;
            flashValue = 2.5f;
            //prevRenderer = cubes[currentTarget].GetComponent<Renderer>();
            //prevRenderer.material.color = Color.red;
            bpmTracker++;
            totalBeats++;
            valueChanged = false;
            combo.text = totalBeats.ToString();
            Debug.Log(currPeak - lowestValue);
            Debug.DrawRay(new Vector2(cubes[0].transform.position.x, prevValue * 50), new Vector2(numberOfObjects, 0), Color.black, 0.1f);
            // Bullet inst = Instantiate(projectile, flash.transform.position, Quaternion.identity);
            //inst.angle = (totalBeats * 15 * Mathf.PI * 2) / 360;
            //Destroy(inst.gameObject, 5);
            //cubesShadow[selectedChannel].transform.localScale = peakScale;
            //cubesShadow[selectedChannel].transform.position = peakHeight;
            //cubesShadow[selectedChannel].material.color = new Color(cubesShadow[selectedChannel].material.color.r, cubesShadow[selectedChannel].material.color.g, cubesShadow[selectedChannel].material.color.b, 1);
        }

        if (currPeak - prevValue < 0)
            lowestValue = currPeak;

        prevValue = currPeak;
        */

        //for (int i = 0; i < cubesShadow.Count; i++)
        //cubesShadow[i].material.color = new Color(cubesShadow[i].material.color.r, cubesShadow[i].material.color.g, cubesShadow[i].material.color.b, cubesShadow[i].material.color.a - 0.01f);


        //if (flashValue > 0) {
        //Color color = Random.ColorHSV();
        //flash.color = new Color(color.r, color.g, color.b, flashValue);
        //flashValue -= 0.1f;
        //}



        if (samples[selectedChannel] < pV[selectedChannel])
            if (samples[selectedChannel] - lV[selectedChannel] >= lV[selectedChannel]) {
                flashValue = 2.5f;
                totalBeats++;
                combo.text = totalBeats.ToString();

                cubesShadow[selectedChannel].transform.position = new Vector3(cubesShadow[selectedChannel].transform.position.x, pV[selectedChannel] * 100);
                Debug.DrawRay(new Vector2(-100, pV[selectedChannel] * 100),new Vector2(200,0),Color.red,1f);
            }




        for (int i = 0; i < actualNumber; i++)
            if (samples[i] < pV[i]) {
                lV[i] = samples[i];
            }
        //else if (hasUpwards) {
        /*hasUpwards = false;
        flashValue = 2.5f;

        totalBeats++;
        combo.text = totalBeats.ToString();*/
        // }

        /*if (samples[cP] < pPV)
            for (int i = 0; i < 2; i++) {
                if (pD[i] != pPD[i]) {
                    Debug.Log("Working");
                    pPV = 0;

                    flashValue = 2.5f;
                    totalBeats++;
                    combo.text = totalBeats.ToString();
                }

                Debug.Log(pPD[i] + " " + pD[i]);
                pPD[i] = pD[i];
            }*/

        if (flashValue > 1.5f) {
            flash.transform.localScale = new Vector3(flashValue, flashValue, 0);
            flashValue -= 0.25f;
        }

        if (song.time >= 60 * currMinute) {
            Debug.LogWarning("Current BPM Tracked: " + bpmTracker);
            bpmTracker = 0;
            currMinute++;
        }

        pV = samples;
    }

    // bool UpdatePrevData(int item, float value) {
    //if (value > prevData[item].prevValue)
    // return true;
    //return false;
    // }
}

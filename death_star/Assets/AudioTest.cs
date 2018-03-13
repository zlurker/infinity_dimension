using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioTest : MonoBehaviour {

    public class VelocityInfo {
        public float timestamp;
    }
    public int numOfSamples = 8192; //Min: 64, Max: 8192

    public AudioSource aSource;

    public float[] freqData;
    public float[] band;
    public int highestBand = 0;

    public GameObject[] g;
    bool test = true;
    float flashValue;
    int totalBeats;
    public Text combo;
    public GameObject flash;
    bool upwards;
    float prevValue;
    int highest = 0;

    void Start() {
        freqData = new float[numOfSamples];

        int n = freqData.Length;

        // checks n is a power of 2 in 2's complement format
        //if ((n*(n - 1)) != 0) {
        // Debug.LogError("freqData length " + n + " is not a power of 2!!! Min: 64, Max: 8192.");
        //return;
        //}

        int k = 0;
        for (int j = 0; j < freqData.Length; j++) {
            n = n / 2;
            if (n <= 0) break;
            k++;
        }

        band = new float[k + 1];
        g = new GameObject[k + 1];


        for (int i = 0; i < band.Length; i++) {
            band[i] = 0;
            g[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g[i].GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            g[i].transform.position = new Vector3(i, 0, 0);

        }

        //InvokeRepeating("check", 0.0f, 1.0f / 15.0f); // update at 15 fps


    }

    void Update() {
        check();

        if (flashValue > 1.5f) {
            flash.transform.localScale = new Vector3(flashValue, flashValue, 0);
            flashValue -= 0.25f;
        }
    }

    private void check() {

        /*
         * 
         * 
         * 
         * 
         * 
         * 
         */

        

        /*
         * 
         * 
         * 
         * 
         * 
         */

        aSource.GetSpectrumData(freqData, 0, FFTWindow.Rectangular);

        int k = 0;
        int crossover = 2;

        for (int i = 0; i < freqData.Length; i++) {
            if (k == 0)
                band[0] = 0;

            float d = freqData[i];
            float b = band[k];

            // find the max as the peak value in that frequency band.
            band[k] = (d > b) ? d : b;
            //Debug.Log(crossover);
            //if (test)
            //Debug.Log(k);
            if (i > (crossover - 3)) {

                if (band[k] > band[highest]) {
                    highest = k;
                    prevValue = band[k];
                }
                

                Vector3 tmp = new Vector3(g[k].transform.position.x, band[k] * 200, g[k].transform.position.z);
                g[k].transform.position = tmp;

                k++;
                crossover *= 2;   // frequency crossover point for each band.              
                band[k] = 0;

            }



        }

        if (band[highest] < prevValue) {
            if (upwards) {
                upwards = false;
            }
        } else
            upwards = true;

        prevValue = band[highest];


    }

    // void Update() { }
}


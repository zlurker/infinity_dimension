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

    public GameObject[] g;

    //------------------------Values below are debug values------------------------//
    int selectedChannel;
    float prevVelocity;
    float prevTimestamp;
    float prevValue;

    void Start() {
        selectedChannel = 7;

        freqData = new float[numOfSamples];

        int n = freqData.Length;

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

        g[selectedChannel].GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    void Update() {
        check();
    }

    private void check() {
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

            if (i > (crossover - 3)) {
                Vector3 tmp = new Vector3(g[k].transform.position.x, band[k] * 200, g[k].transform.position.z);
                g[k].transform.position = tmp;

                k++;
                crossover *= 2;   // frequency crossover point for each band.              
                band[k] = 0;
            }
        }

        //------Debug values-------//
        float timeDifference = aSource.time - prevTimestamp;

        if (timeDifference != 0) {
            float currVelocity = (band[selectedChannel] - prevValue) / timeDifference;

            if (prevVelocity > 0 && currVelocity < 0)
                Debug.LogFormat("PrevVelocity: {0}, CurrVelocity: {1}", prevVelocity, currVelocity);

            if (currVelocity == Mathf.Infinity)
                Debug.LogFormat("Values that caused infinity: Difference: {0}, Time: {1}", band[selectedChannel] - prevValue, aSource.time - prevTimestamp);

            prevVelocity = currVelocity;
            prevTimestamp = aSource.time;
            prevValue = band[selectedChannel];
        }
    }
}


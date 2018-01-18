using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Wave;
using System;
using System.IO;
public class TestMusicLoad : MonoBehaviour {

    public AudioClip test;
    public AudioSource t;
    float[] data;
    string fileName = "C:/Users/zlurker/Desktop/UnityProjects/ClusterPrism/Cluster Prism/Assets/Music/Clean Bandit - Symphony feat. Zara Larsson ( Lyrics _ Lyric Video ).mp3";

    public int samplerate = 44100;
    public float frequency = 440;
    // Use this for initialization
    void Start() {
        //test = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, false);
        Test();
        //test = AudioClip.Create("Test", samplerate * 2, 1, samplerate, true);
        //test.SetData(Convert16BitToFloat(File.ReadAllBytes(fileName)),0);
        t.clip = test;
        t.Play();
    }

    // Update is called once per frame
    void Update() {

    }

    public float[] Convert16BitToFloat(byte[] input) {
        int inputSamples = input.Length / 2; // 16 bit input, so 2 bytes per sample
        float[] output = new float[inputSamples];
        int outputIndex = 0;
        for (int n = 0; n < inputSamples; n++) {
            short sample = BitConverter.ToInt16(input, n * 2);
            output[outputIndex++] = sample / 32768f;
        }
        return output;
    }

    void Test() {

        var file = new Mp3FileReader(fileName);
        int _Bytes = (int)file.Length;
        byte[] Buffer = new byte[_Bytes];

        int read = file.Read(Buffer, 0, (int)_Bytes);
        data = new float[read];


        Wave16ToFloatProvider t = new Wave16ToFloatProvider(file);
        int j = 0;
        for (int i = 0; i < read; i += 4) {
            float leftSample = BitConverter.ToInt16(Buffer, i);
            float rightSample = BitConverter.ToInt16(Buffer, i + 2);

            leftSample = leftSample / 32768f;
            rightSample = rightSample / 32768f;

            data[i / 2] = leftSample;
            data[(i / 2) + 1] = rightSample;
            /*if (j < 1000)
                if (leftSample != 0 || rightSample != 0) {
                    Debug.Log(leftSample + " " + rightSample);
                    j++;
                }*/
        }

        test.SetData(data, 0);
    }
}

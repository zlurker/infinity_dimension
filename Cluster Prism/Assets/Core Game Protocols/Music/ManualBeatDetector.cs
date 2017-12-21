using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ManualBeatDetector : MonoBehaviour {

    AudioSource music;
    List<float> beatTimings;
    int calibration;
    public int calibrationRequired;

    void Start() {
        GlobalData.analyseAlgorithm = RhythmAnalyseState.Manual;

        music = GetComponent<AudioSource>();
        GlobalData.song = music.clip;
        beatTimings = new List<float>();

        PlayerInput.i.AddNewInput(KeyCode.L, new DH(Load));
        PlayerInput.i.AddNewInput(0, new DH(RegisterClick));
        PlayerInput.i.AddNewInput(KeyCode.Escape, new DH(EraseData));
    }

    void CalculateBPM(float timing) {
        float avgBpm = 0;
        beatTimings.Add(timing);
        for (int i = 0; i < beatTimings.Count; i++) {
            if (i != 0)
                avgBpm += beatTimings[i] - beatTimings[i - 1];
        }

        avgBpm /= (beatTimings.Count - 1);
        float avgOffset = 0;

        for (int i = 0; i < beatTimings.Count; i++)
            avgOffset += beatTimings[i] % avgBpm;

        avgOffset /= beatTimings.Count;

        GlobalData.bpm = 60 / avgBpm;
        GlobalData.offset = avgOffset;

        Debug.LogFormat("Current BPM is {0}. Offset is {1} ", 60 / avgBpm, avgOffset);
    }

    void Load(object[] p) {
        GlobalData.LoadNewLevel(2);
    }

    void RegisterClick(object[] p) {
        if (music.isPlaying) {
            calibration++;

            if (calibration > calibrationRequired)
                CalculateBPM(music.time);
        }
    }

    void EraseData(object[] p) {
        beatTimings = new List<float>();
        calibration = 0;
    }
}

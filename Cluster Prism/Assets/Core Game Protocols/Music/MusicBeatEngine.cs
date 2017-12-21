using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicBeatEngine : MonoBehaviour {

    public float distMultiplier = 1;
    public GameObject beat;
    public float[] beatTimings;

    AudioSource music;
    float totalDist;

    public JudgementRange[] jR;
    int cB;

    void Start() {
        music = GetComponent<AudioSource>();
        music.clip = GlobalData.song;

        switch (GlobalData.analyseAlgorithm) {
            case RhythmAnalyseState.Manual:
                int numberOfBeats = Mathf.FloorToInt(GlobalData.bpm * (music.clip.length / 60));

                Debug.Log(numberOfBeats + " " + music.clip.length / 60 + " " + GlobalData.bpm);

                beatTimings = new float[numberOfBeats];

                for (int i = 0; i < numberOfBeats; i++) {
                    Instantiate(beat, new Vector3(0, i * distMultiplier, 0), Quaternion.identity).transform.parent = transform;
                    beatTimings[i] = ((i / GlobalData.bpm) * 60);
                }

                totalDist = (GlobalData.bpm * (music.clip.length / 60)) * distMultiplier;
                break;
        }

        jR = (JudgementRange[])PresetGameplayData.jRT.Clone();
        PlayerInput.i.AddNewInput(0, new DH(BeatJudgeProcessing));

        music.Play();
    }


    void Update() { //Judgement somewhat buggy. Need to analyse more.
        transform.position = new Vector3(0, -(((music.time - GlobalData.offset) / music.clip.length) * totalDist), 0);

        /*if (music.time - GlobalData.offset >= beatTimings[cB])//Input.GetMouseButtonDown(0)) {
            for (int i = 0; i < jR.Length; i++)
                if (Mathf.Abs(music.time - beatTimings[cB] - GlobalData.offset) <= jR[i].maxWindow) {
                    UIDrawer.i.UpdateGraphic("BeatGrade", jR[i].name, 0.25f);
                    jR[i].counter++;
                    cB++;
                }
        //}*/

        if (music.time - GlobalData.offset >= beatTimings[cB] + jR[jR.Length - 1].maxWindow) {
            //UIDrawer.i.UpdateGraphic("BeatGrade", "X", 0.25f);
            cB++;
        }
    }

    void BeatJudgeProcessing(object[] p) {
        float tDFB = Mathf.Abs(music.time - beatTimings[cB] - GlobalData.offset);

        for (int i = 0; i < jR.Length; i++)
            if (tDFB <= jR[i].maxWindow) {

                for (int j = 0; j < DelegatePools.jD.Count; j++)
                    DelegatePools.jD[j].Invoke(new object[] { i });

                //UIDrawer.i.UpdateGraphic("BeatGrade", jR[i].name, 0.25f);
                jR[i].counter++;
                cB++;
                return;
            }
    }
}

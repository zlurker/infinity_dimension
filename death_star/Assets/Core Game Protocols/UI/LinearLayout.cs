using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLayout : MonoBehaviour, IOnSpawn
{
    public enum Orientation
    {
        X, Y
    }

    public List<RectTransform> objects;
    public Vector3 sizeConstrain;
    public Vector3 multiplier;
    public Orientation o;

    public void OnSpawn() {
        o = Orientation.Y;
        multiplier = new Vector2(1, -1);
        objects = new List<RectTransform>();
        (transform as RectTransform).sizeDelta = new Vector2(0, 0);
        (transform as RectTransform).pivot = new Vector2(0, 1);

        
    } 

    public void Add(RectTransform target) {
        objects.Add(target);
        target.SetParent(transform);

        
        int altIndex = (int)o == 1 ? 0 : 1;
        sizeConstrain[altIndex] = sizeConstrain[altIndex] < target.sizeDelta[altIndex] ? target.sizeDelta[altIndex] : sizeConstrain[altIndex];
        sizeConstrain[(int)o] += target.sizeDelta[(int)o];

        //SlotItemIn(target);
        Debug.LogFormat("Actual {0}, Alternate {1}", (int)o, altIndex);
        Debug.LogFormat("Size Constraint: {0}, Target SD: {1}, SC SD: {2}",sizeConstrain, target.sizeDelta[altIndex], sizeConstrain[altIndex]);
        RecalculateBounds();
    }

    void RecalculateBounds() {
        (transform as RectTransform).sizeDelta = sizeConstrain;
        sizeConstrain[(int)o] = 0;

        for(int i = 0; i < objects.Count; i++)
            SlotItemIn(objects[i]);
    }

    void SlotItemIn(RectTransform target)
    {       
        Vector3 lengthAddition = new Vector3();

        lengthAddition[(int)o] = sizeConstrain[(int)o] * multiplier[(int)o];
        
        Vector3 finalPos = new Vector3();
        int altIndex = (int)o == 1 ? 0 : 1;

        finalPos[altIndex] = target.pivot[altIndex] * target.sizeDelta[altIndex] * multiplier[altIndex];
        finalPos[(int)o] = lengthAddition[(int)o] + (target.pivot[(int)o] * target.sizeDelta[(int)o] * multiplier[(int)o]);
        target.localPosition = finalPos;

        /* Adds the targeted object's targeted dimension into the sizeconstrain
        to be tracked for future calculations.*/
        sizeConstrain[(int)o] += target.sizeDelta[(int)o];
    }   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLayout : MonoBehaviour, IAddOn, ISpawn
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
    } 

    public void Add(RectTransform target) {
        objects.Add(target);
        target.SetParent(transform);
        RecalculateBounds();
    }

    void RecalculateBounds() {
        sizeConstrain = new Vector3();

        for(int i = 0; i < objects.Count; i++)
            SlotItemIn(objects[i]);

        (transform as RectTransform).sizeDelta = sizeConstrain;
    }

    void SlotItemIn(RectTransform target)
    {
        int altIndex = (int)o == 1 ? 0 : 1;
        Vector3 lengthAddition = new Vector3();

        //target.pivot = new Vector2(0,0.5f);//need to take pivot into account.
        lengthAddition[(int)o] = sizeConstrain[(int)o] * multiplier[(int)o];
        sizeConstrain[altIndex] = sizeConstrain[altIndex] < target.sizeDelta[altIndex] ? target.sizeDelta[altIndex] : sizeConstrain[altIndex];
        Vector3 finalPos = new Vector3();
        finalPos[(int)o] = lengthAddition[(int)o];
        target.localPosition = finalPos;

        sizeConstrain[(int)o] += target.sizeDelta[(int)o];
    }


    
}

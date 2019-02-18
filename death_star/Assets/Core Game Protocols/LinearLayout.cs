using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLayout : MonoBehaviour, IAddOn, ISpawn
{
    public enum Orientation
    {
        X, Y
    }

    public Vector3 sizeConstrain;
    public Vector3 multiplier;
    public Orientation o;
    public Group g;

    // Update is called once per frame
    void Update()
    {

    }

    public void Add(object target)
    {
        //Debug.Log("Interface is working");
        if (target is Group){
            Vector3 groupLength = Vector3.zero;
            Group g = target as Group;

            LinearLayout lL = null;

            if (Iterator.ReturnObject<LinearLayout>(g.aO.ToArray()) != null)
                lL = (Iterator.ReturnObject<LinearLayout>(g.aO.ToArray()) as AddOnData).i as LinearLayout;

            if (lL)
                groupLength = lL.sizeConstrain;
            
            SlotItemIn(g.gP.transform, groupLength);
        }

        if (target is ScriptableObject){
            Vector3 targetLength = new Vector3();
            ScriptableObject sO = target as ScriptableObject;

            if (sO.transform is RectTransform)
                targetLength = (sO.transform as RectTransform).sizeDelta;

            SlotItemIn(sO.transform, targetLength);
        }
    }

    public void SlotItemIn(Transform target, Vector3 l)
    {
        int altIndex = (int)o == 1 ? 0 : 1;
        Vector3 lengthAddition = new Vector3();

        lengthAddition[(int)o] = sizeConstrain[(int)o] * multiplier[(int)o];
        sizeConstrain[altIndex] = sizeConstrain[altIndex] < l[altIndex] ? l[altIndex] : sizeConstrain[altIndex];
        Vector3 finalPos = target.localPosition;
        finalPos[(int)o] = lengthAddition[(int)o];
        target.localPosition = finalPos;

        sizeConstrain[(int)o] += l[(int)o];
    }

    public void LinkedGroup(Group group)
    {
        g = group;
    }

    public void OnSpawn()
    {
        o = Orientation.Y;
        multiplier = new Vector2(1, -1);
    }
}

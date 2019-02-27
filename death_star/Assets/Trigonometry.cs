using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Trigonometry : MonoBehaviour, IDragHandler {

    // Use this for initialization
    public RectTransform target;
    public Camera test;
    public Canvas canvas;
    public Image targetGraphic;
    bool dragged;
    Vector3 diff;

    void Start() {
        canvas = GetComponent<Canvas>();
        Debug.Log(Math.CalculateAngle(new Vector3(-1f, -0.5f)));

    }

    void Update() {
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 wP;
        Vector3 t;
        test = eventData.pressEventCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, eventData.pressEventCamera, out wP);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, Input.mousePosition, eventData.pressEventCamera, out t);
        Debug.Log(t + " " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2 d = eventData.position - RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, target.position);
        diff = new Vector3(wP.x, wP.y) - target.localPosition;
        
        Debug.Log(d + " " + d.magnitude);

        target.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(diff)));
        (target.transform.GetChild(0) as RectTransform).sizeDelta = new Vector2(40f, d.magnitude * canvas.scaleFactor);
        dragged = true;
        //Debug.Log("TargetPos: " + transform.localPosition);
    }
}

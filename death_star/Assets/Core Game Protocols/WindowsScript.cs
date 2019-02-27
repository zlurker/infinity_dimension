using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class WindowsScript : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {

    public Image hotspot;
    Vector2 pointInObject;
    Vector3 trackedPos;

    IPointerDownHandler test;
    
    // Use this for initialization
    void Start () {
        (transform.parent as RectTransform).pivot = new Vector2(0.5f, 1);

        hotspot = GetComponent<Image>();

        Color originalColor = hotspot.color;
        originalColor.a = 0;
        hotspot.color = originalColor;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out pointInObject);            
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currMousePos; 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out currMousePos);
        Debug.Log(currMousePos);
        transform.parent.localPosition = currMousePos - pointInObject;
    }
}

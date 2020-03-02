using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*public interface IWindowsDragEvent {
    void OnDrag();
}*/

[RequireComponent(typeof(Image))]
public class WindowsScript : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler, IOnSpawn {

    Image hotspot;
    Vector2 pointInObject;
    Vector3 trackedPos;

    public virtual void OnPointerDown(PointerEventData eventData) {
        transform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out pointInObject);
    }

    public virtual void OnPointerUp(PointerEventData eventData) {

    }

    public virtual void OnDrag(PointerEventData eventData) {
        Vector2 currMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out currMousePos);
        transform.localPosition = currMousePos - pointInObject;
    }

    public void OnSpawn() {
        transform.SetAsLastSibling();
        (transform as RectTransform).sizeDelta = new Vector2(100, 40);
    }
}

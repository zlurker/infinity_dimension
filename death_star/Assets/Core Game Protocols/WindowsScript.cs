using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*public interface IWindowsDragEvent {
    void OnDrag();
}*/

[RequireComponent(typeof(Image))]
public class WindowsScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public Image hotspot;
    Vector2 pointInObject;
    Vector3 trackedPos;

    public virtual void OnPointerDown(PointerEventData eventData) {
        transform.parent.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out pointInObject);
    }

    public virtual void OnPointerUp(PointerEventData eventData) {

    }

    public virtual void OnDrag(PointerEventData eventData) {
        Vector2 currMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out currMousePos);
        transform.parent.localPosition = currMousePos - pointInObject;
    }
}

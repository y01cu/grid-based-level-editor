using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTest : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("mouse is here", gameObject);
    }
}

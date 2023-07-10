using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        DisableRaycastTargetsRecursively(transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        EnableRaycastTargetsRecursively(transform);
    }
    private void DisableRaycastTargetsRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Graphic graphic = child.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }

            DisableRaycastTargetsRecursively(child);
        }
    }
    private void EnableRaycastTargetsRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Graphic graphic = child.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }

            EnableRaycastTargetsRecursively(child);
        }
    }
}

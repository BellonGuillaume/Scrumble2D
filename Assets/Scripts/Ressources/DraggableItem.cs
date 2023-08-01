using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public UserStory userStory;
    [HideInInspector] public UserStoryUI userStoryUI;
    [HideInInspector] public bool isDragged; 
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD && userStory.state == UserStory.State.PRODUCT_BACKLOG && userStoryUI.canBeDrag){
            parentAfterDrag = transform.parent;
            transform.parent.GetComponent<DropCase>().userStoryUI = null;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            DisableRaycastTargetsRecursively(transform);
            isDragged = true;
            EventManager.movingUS = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD && userStory.state == UserStory.State.PRODUCT_BACKLOG && userStoryUI.canBeDrag){
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD && userStory.state == UserStory.State.PRODUCT_BACKLOG && userStoryUI.canBeDrag){
            if (parentAfterDrag.childCount > 0){
                parentAfterDrag.GetComponent<DropCase>().MoveToNext(this.userStoryUI);
                EnableRaycastTargetsRecursively(transform);
                isDragged = false;
            } else {
                transform.SetParent(parentAfterDrag);
                transform.parent.GetComponent<DropCase>().userStoryUI = this.userStoryUI;
                EnableRaycastTargetsRecursively(transform);
                isDragged = false;
            }
            EventManager.movingUS = false;
        }
    }
    private void DisableRaycastTargetsRecursively(Transform parent)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD && userStory.state == UserStory.State.PRODUCT_BACKLOG && userStoryUI.canBeDrag){
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
    }
    private void EnableRaycastTargetsRecursively(Transform parent)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD && userStory.state == UserStory.State.PRODUCT_BACKLOG && userStoryUI.canBeDrag){
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
}

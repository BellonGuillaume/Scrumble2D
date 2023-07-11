using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropCase : MonoBehaviour, IDropHandler
{
    [HideInInspector] public UserStoryUI userStoryUI;
    [HideInInspector] public DropContent dropContent;
    
    public void OnDrop(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD){
            if (eventData.pointerDrag.GetComponent<DraggableItem>() == null){
                return;
            }
            if (transform.childCount == 0){
                GameObject dropped = eventData.pointerDrag;
                dropped.GetComponent<DraggableItem>().parentAfterDrag = transform;
                userStoryUI = dropped.GetComponent<UserStoryUI>();
            } else {
                dropContent.OnDrop(eventData);
            }
        }
    }
    public void AddUsUI(GameObject userStoryUI){
        if (transform.childCount == 0){
            userStoryUI.transform.SetParent(this.transform);
            this.userStoryUI = userStoryUI.GetComponent<UserStoryUI>();
        }
    }
}

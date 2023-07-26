using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropCase : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public UserStoryUI userStoryUI;
    [HideInInspector] public DropContent dropContent;
    public DropCase previousDropCase;
    public DropCase nextDropCase;
    
    public void OnDrop(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD){
            if (eventData.pointerDrag.GetComponent<DraggableItem>() == null){
                return;
            }
            if (dropContent == null){
                return;
            }
            if (transform.childCount == 0){
                GameObject dropped = eventData.pointerDrag;
                dropped.GetComponent<DraggableItem>().parentAfterDrag = transform;
                userStoryUI = dropped.GetComponent<UserStoryUI>();
            } else {
                if (dropContent != null){
                    dropContent.OnDrop(eventData);
                }
            }
        }
    }
    public void AddUsUI(GameObject userStoryUI){
        if (transform.childCount == 0){
            userStoryUI.transform.SetParent(this.gameObject.transform);
            this.userStoryUI = userStoryUI.GetComponent<UserStoryUI>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null)
            return;
        if (eventData.pointerDrag.GetComponent<DraggableItem>() is null)
            return;
        if (eventData.pointerDrag.GetComponent<DraggableItem>().isDragged == false)
            return;
        if (this.transform.childCount > 0){
            if(nextDropCase is null)
                return;
            this.nextDropCase.MoveToNext(this.transform.GetChild(0), this.userStoryUI);
            this.userStoryUI = null;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null)
            return;
        if (eventData.pointerDrag.GetComponent<DraggableItem>() is null)
            return;
        if (this.nextDropCase is null)
            return;
        if (eventData.pointerDrag.GetComponent<DraggableItem>().isDragged == false)
            return;
        if (this.transform.childCount == 0){
            this.nextDropCase.MoveToPrev();
        }
    }
    public void MoveToNext(Transform card, UserStoryUI userStory){
        if(this.transform.childCount == 0){
            card.SetParent(this.transform);
            this.userStoryUI = userStory;
            return;
        }
        if(this.nextDropCase is null){
        } else {
            this.nextDropCase.MoveToNext(this.transform.GetChild(0), this.userStoryUI);
            card.SetParent(this.transform);
            this.userStoryUI = userStory;
        }
    }
    public void MoveToPrev(){
        if(this.transform.childCount == 0){
            this.previousDropCase.userStoryUI = null;
            return;
        }
        this.transform.GetChild(0).transform.SetParent(this.previousDropCase.transform);
        this.previousDropCase.userStoryUI = this.userStoryUI;
        if(this.nextDropCase is null){
            this.userStoryUI = null;
            return;
        }
        this.nextDropCase.MoveToPrev();
    }
}

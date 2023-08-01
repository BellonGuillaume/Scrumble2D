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
            this.MoveToNext(null);
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
            this.GetFromNext();
        }
    }
    public void MoveToNext(UserStoryUI newUS){
        if (this.transform.childCount == 0){
            this.userStoryUI = newUS;
            if (newUS is not null)
                newUS.transform.SetParent(this.transform);
        } else {
            UserStoryUI oldUS = this.userStoryUI;
            this.userStoryUI = newUS;
            if (newUS is not null)
                newUS.transform.SetParent(this.transform);
            this.nextDropCase.MoveToNext(oldUS);
        }
    }
    public UserStoryUI GetNextUS(){
        if (this.userStoryUI is null){
            if (this.nextDropCase is null)
                return null;
            else
                return this.nextDropCase.GetNextUS();
        } else {
            UserStoryUI usToReturn = this.userStoryUI;
            this.userStoryUI = null;
            return usToReturn;
        }
    }

    public void GetFromNext(){
        if (this.nextDropCase is null)
            return;
        UserStoryUI newUS = this.GetNextUS();
        if (newUS is not null)
            newUS.transform.SetParent(this.transform);
        this.userStoryUI = newUS;
        this.nextDropCase.GetFromNext();
    }
}

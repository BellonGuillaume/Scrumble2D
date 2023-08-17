using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HighlightScore : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject back;
    [SerializeField] TMP_Text score;
    [HideInInspector] public UserStory userStory;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.score.text = this.userStory.currentTask.ToString() + " / " + this.userStory.maxTask.ToString();
        this.back.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.back.SetActive(false);
    }
}

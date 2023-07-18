using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DaySlider : MonoBehaviour
{
    public void SetAlpha(float alpha){
        Image[] images = GetComponentsInChildren<Image>();
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        Color newColor;
        foreach(Image image in images){
            newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
        }
        foreach(TMP_Text text in texts){
            newColor = text.color;
            newColor.a = alpha;
            text.color = newColor;
        }
    }
}

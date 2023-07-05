using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoSizeText : MonoBehaviour
{
    public RectTransform imageRect;
    TMP_Text textComponent;
    public float defaultImageWidth;
    public float defaultImageHeight;
    public Vector2 defaultPos;
    float scaleFactorX;
    float scaleFactorY;
    
    void Awake(){
        this.textComponent = this.GetComponent<TMP_Text>();
    }

    void Update(){
        if (imageRect != null){
            scaleFactorX = imageRect.rect.width / defaultImageWidth;
            scaleFactorY = imageRect.rect.height / defaultImageHeight;

            textComponent.rectTransform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
            textComponent.rectTransform.localPosition = new Vector3(defaultPos.x * scaleFactorX, defaultPos.y * scaleFactorY, 0f);
        }
    }
    
}

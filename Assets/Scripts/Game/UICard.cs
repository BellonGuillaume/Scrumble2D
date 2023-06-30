using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour
{
    public TMP_Text description;
    public TMP_Text result;
    public Image recto;
    public Image verso;

    bool isRecto;

    void Start(){
        isRecto = false;
    }

    public void flipCard(){
        if(isRecto){
            recto.enabled = false;
            verso.enabled = true;
            isRecto = false;
        } else {
            recto.enabled = true;
            verso.enabled = false;
            isRecto = true;
        }
    }

    public UICard Click(){
        return this;
    }

    public override string ToString()
    {
        return $"Carte {this.isRecto}. Face Visible : {recto.color}";
    }

}

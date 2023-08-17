using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Results : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public static bool initialized;

    public void ChangeText(string text){
        this.text.text = text;
        initialized = true;
    }

    public void Reset(){
        this.text.text = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrueFalseScrollbar : MonoBehaviour
{
    public Slider slider;

    public void Click(){
        if (slider.value == 1){
            slider.value = 0;
            Debug.Log("Valeur changée à 0");
        } else {
            slider.value = 1;
            Debug.Log("Valeur changée à 1");
        }
    }
}

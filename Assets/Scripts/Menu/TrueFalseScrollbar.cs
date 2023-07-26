using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrueFalseScrollbar : MonoBehaviour
{
    public Slider slider;
    public Image handle;
    public Image emptyBack;
    public Image filledBack;

    void Update(){
        if (slider.value == 0){
            this.handle.color = Color.red;
            this.emptyBack.color = Color.red;
            this.filledBack.color = Color.red;
        } else {
            this.handle.color = Color.green;
            this.emptyBack.color = Color.green;
            this.filledBack.color = Color.green;
        }
    }

    public void Click(){
        if (slider.value == 1){
            slider.value = 0;
        } else {
            slider.value = 1;
        }
    }
}

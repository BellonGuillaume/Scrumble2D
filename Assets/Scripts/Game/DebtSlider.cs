using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DebtSlider : MonoBehaviour
{
    [SerializeField] TMP_Text counter;

    public void OnValueChanged(Single single){
        counter.text = Mathf.RoundToInt(single).ToString();
    }
}
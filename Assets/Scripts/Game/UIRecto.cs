using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRecto : MonoBehaviour
{
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text result;
    [SerializeField] Image backGround;
    public int id;

    void Start()
    {

    }

    public override string ToString()
    {
        return $"Recto with description {description} and result {result}";
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

public class DropDownLocalize : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] LocalizedString[] options;
    void Start()
    {
        dropdown.ClearOptions();
        var localizedOptions = new List<string>();
        foreach (var option in options){
            string localizedText = option.GetLocalizedString();
            localizedOptions.Add(localizedText);
        }
        dropdown.AddOptions(localizedOptions);
    }
}

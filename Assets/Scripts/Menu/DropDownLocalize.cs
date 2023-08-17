using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Settings;

public class DropDownLocalize : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] LocalizedString[] options;

    private Locale language;
    void Start(){
        language = LocalizationSettings.AvailableLocales.GetLocale("en");
    }

    void Update(){
        if (language != LocalizationSettings.SelectedLocale){
            int selectedValue = dropdown.value;
            dropdown.ClearOptions();
            var localizedOptions = new List<string>();
            foreach (var option in options){
                string localizedText = option.GetLocalizedString();
                localizedOptions.Add(localizedText);
            }
            dropdown.AddOptions(localizedOptions);
            dropdown.value = selectedValue;
            language = LocalizationSettings.SelectedLocale;
        }
        
    }
}

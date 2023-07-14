using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Menu : MonoBehaviour
{
    void Start()
    {
        StateManager.language = LocalizationSettings.SelectedLocale;
    }

    public void OnFrenchClick(){
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        StateManager.language = LocalizationSettings.SelectedLocale;
    }
    public void OnEnglishClick(){
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        StateManager.language = LocalizationSettings.SelectedLocale;
    }
}

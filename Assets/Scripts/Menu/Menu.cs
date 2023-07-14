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
}

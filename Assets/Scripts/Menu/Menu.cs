using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] GameObject settingsPannel;
    [SerializeField] Image settingsBack;
    [SerializeField] GameObject settingsElements;
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

    public void OnManualClick(){
        string pdfManualPath = Path.Combine(Application.streamingAssetsPath, "Manual/UserManual.pdf");
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start(pdfManualPath);
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        System.Diagnostics.Process.Start("open", pdfManualPath);
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        System.Diagnostics.Process.Start("xdg-open", pdfManualPath);
#else
        Debug.LogError("Plate-forme non prise en charge pour l'ouverture de PDF.");
#endif
    }

    public void OnSettingsClick(){
        Color32 temp = settingsBack.color;
        temp.a = 0;
        settingsBack.color = temp;
        Vector2 startScale = Vector2.zero;
        Vector2 endScale = Vector2.one;
        byte startAlpha = 0;
        byte endAlpha = 169;
        settingsPannel.SetActive(true);
        this.CreateAnimationRoutine(
            0.3f,
            delegate (float progress){
                float easedProgress = Easing.easeInQuad(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte alpha = (byte) Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                Color32 temp = settingsBack.color;
                temp.a = alpha;
                settingsElements.transform.localScale = scale;
                settingsBack.color = temp;
            }
        );
    }

    public void OnExitClick(){
        Vector2 startScale = Vector2.one;
        Vector2 endScale = Vector2.zero;
        byte startAlpha = 169;
        byte endAlpha = 0;
        this.CreateAnimationRoutine(
            0.3f,
            delegate (float progress){
                float easedProgress = Easing.easeOutQuad(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte alpha = (byte) Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                Color32 temp = settingsBack.color;
                temp.a = alpha;
                settingsElements.transform.localScale = scale;
                settingsBack.color = temp;
            },
            delegate{
                settingsPannel.SetActive(false);
            }
        );
    }

    public void OnHyperlinkClick(){
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale("fr"))
            Application.OpenURL("http://scrumble.pyxis-tech.com/index_fr.html");
        else
            Application.OpenURL("http://scrumble.pyxis-tech.com/");
    }
}

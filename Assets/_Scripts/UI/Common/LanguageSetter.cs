using TMPro;
using UnityEngine;
using YG;

public class LanguageSetter : MonoBehaviour
{
    private TMP_Dropdown dropDown;
    
    private void OnEnable()
    {
        if (YandexGame.SDKEnabled)
            Init();
        else
            YandexGame.GetDataEvent += Init;
        
        void Init()
        {
            if(dropDown == null)
                dropDown = GetComponent<TMP_Dropdown>();
            
            if (YandexGame.savesData.languageId <= -1)
            {
                var languageId = GetLanguageId(YandexGame.EnvironmentData.language); 

                dropDown.value = GetTrueLanguageIdD(languageId);
                YandexGame.savesData.languageId = dropDown.value;
                
                CurrentLanguageData.instance.SetLanguageData(languageId);
            }
            else
                dropDown.value = YandexGame.savesData.languageId;
            
            Work(YandexGame.savesData.languageId);
        }
    }

    private void Start()
    {
        if(dropDown != null)
            dropDown.onValueChanged.AddListener(Work);
    }

    private void Work(int id)
    {
        YandexGame.savesData.languageId = id;
        CurrentLanguageData.instance.SetLanguageData(GetTrueLanguageId(id));
    }
    
    private int GetLanguageId(string textId)
    {
        return textId switch
        {
            "en" => 0,
            "ru" => 1,
            "ar" => 0,
            "ca" => 0,
            "es" => 4,
            "hi" => 0,
            "it" => 6,
            "ko" => 0,
            "ro" => 8,
            "th" => 0,
            "uz" => 10,
            "az" => 1,
            "cs" => 12,
            "fa" => 0,
            "hu" => 14,
            "ja" => 0,
            "nl" => 16,
            "tk" => 0,
            "vi" => 0,
            "be" => 1,
            "de" => 20,
            "fr" => 21,
            "hy" => 1,
            "ka" => 1,
            "pl" => 24,
            "sk" => 0,
            "tr" => 26,
            "zh" => 0,
            "bg" => 28,
            "he" => 0,
            "id" => 0,
            "pt" => 0,
            "sr" => 0,
            "uk" => 1,
            "kk" => 1,
            _ => 0
        };
    }
        
    private int GetTrueLanguageId(int value)
    {
        return value switch
        {
            0 => 0,
            1 => 1,
            2 => 4,
            3 => 6,
            4 => 8,
            5 => 10,
            6 => 12,
            7 => 14,
            8 => 16,
            9 => 20,
            10 => 21,
            11 => 24,
            12 => 28,
            13 => 26,
            _ => 0
        };
    }
    
    private int GetTrueLanguageIdD(int value)
    {
        return value switch
        {
            0 => 0,
            1 => 1,
            5 => 2,
            6 => 3,
            8 => 4,
            10 => 5,
            12 => 6,
            14 => 7,
            16 => 8,
            20 => 9,
            21 => 10,
            24 => 11,
            28 => 12,
            26 => 13,
            _ => 0
        };
    }

}

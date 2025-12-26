using TMPro;
using UnityEngine;
using YG;

public class QualitySetter : MonoBehaviour
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

            dropDown.value = YandexGame.savesData.qualityId;
            
            Work(YandexGame.savesData.qualityId);
        }
    }

    private void Start()
    {
        if(dropDown != null)
            dropDown.onValueChanged.AddListener(Work);
    }

    private void Work(int id)
    {
        QualitySettings.SetQualityLevel(id);
        YandexGame.savesData.qualityId = id;
    }

}

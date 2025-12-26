using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class NewMobileManageSetter : MonoBehaviour
{
    [SerializeField] private Toggle toggler;

    private void Awake()
    {
        if (YandexGame.SDKEnabled)
            Init();
        else
            YandexGame.GetDataEvent += Init;

        void Init()
        {
            var newMenuEnable = YandexGame.savesData.mobileManageAlternativeMenu;

            toggler.isOn = newMenuEnable;
            
            toggler.onValueChanged.AddListener((arg0 =>
            {
                YandexGame.savesData.mobileManageAlternativeMenu = arg0;
            }));
        }
    }

    private void UpdateToggle()
    {
        if (YandexGame.SDKEnabled)
            Init();
        else
            YandexGame.GetDataEvent += Init;

        void Init()
        {
            var newMenuEnable = YandexGame.savesData.mobileManageAlternativeMenu;

            toggler.isOn = newMenuEnable;
        }
    }

    private void OnEnable()
    {
        UpdateToggle();
    }
}

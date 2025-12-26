using System;
using UnityEngine;
using YG;

public class NewMobileManageSwapper : MonoBehaviour
{
    [SerializeField] private GameObject menu1;
    [SerializeField] private GameObject menu2;

    private void OnEnable()
    {
        if (YandexGame.SDKEnabled)
            Init();
        else
            YandexGame.GetDataEvent += Init;

        void Init()
        {
            var newMenuEnable = YandexGame.savesData.mobileManageAlternativeMenu;
            
            menu1.SetActive(!newMenuEnable);
            menu2.SetActive(newMenuEnable);
        }
    }
}

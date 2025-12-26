using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class OnMobileDelte : MonoBehaviour
{
    private void Awake()
    {
        if(YandexGame.EnvironmentData.isMobile)
            Destroy(gameObject);
    }
}

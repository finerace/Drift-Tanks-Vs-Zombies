using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class AuthPlayerPanel : MonoBehaviour
{
    [SerializeField] private GameObject authPanel;

    [Space] 
    
    [SerializeField] private RawImage playerImage;
    [SerializeField] private TMP_Text playerNameLabel;

    [Space] 
    
    [SerializeField] private bool isEnabled = false;
    
    private ImageLoadYG imageLoadYg;
    private bool isAuth;
    
    private void Awake()
    {
        imageLoadYg = FindObjectOfType<ImageLoadYG>();
        
        if(!YandexGame.SDKEnabled) 
            YandexGame.GetDataEvent += SetPlayer;
        else
            SetPlayer();
    }

    private void OnEnable()
    {
        if(isAuth)
            return;
        
        if(isEnabled)
            authPanel.SetActive(true);

        StartCoroutine(CheckAuth());
        IEnumerator CheckAuth()
        {
            while (true)
            {
                if (isAuth)
                    yield break;

                SetPlayer();

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    private void SetPlayer()
    {
        if(!YandexGame.auth || isAuth)
            return;

        playerNameLabel.text = YandexGame.playerName;

        if (imageLoadYg == null)
            FindObjectOfType<ImageLoadYG>();
        
        if (imageLoadYg.rawImage.texture == null)
        {
            imageLoadYg.Load(YandexGame.playerPhoto);
        }

        if (imageLoadYg.rawImage.texture != null)
        {
            playerImage.texture = imageLoadYg.rawImage.texture;
            
            isAuth = true;
        }

        if(isEnabled)
            authPanel.SetActive(false);
    }

    public void Auth()
    {
        YandexGame.AuthDialog();
    }
    
    
}

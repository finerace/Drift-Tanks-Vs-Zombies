using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using YG;

public class AudioSliderSetter : MonoBehaviour
{

    private Slider slider;
    [SerializeField] private AudioMixerGroup target;
    [SerializeField] private string targetVolume;

    [Space] 
    
    [SerializeField] private bool isMasterVolume;

    private void OnEnable()
    {
        if (YandexGame.SDKEnabled)
            Init();
        else
            YandexGame.GetDataEvent += Init;
        
        void Init()
        {
            if(slider == null)
                slider = GetComponent<Slider>();

            slider.value = isMasterVolume ? YandexGame.savesData.masterVolume : YandexGame.savesData.musicVolume;
            
            SetNewVolume(slider.value);
        }
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(SetNewVolume);
    }
    
    private void SetNewVolume(float volume)
    {
        var result = Mathf.Lerp(-80, 20, volume);
            
        target.audioMixer.SetFloat(targetVolume, result);

        if(isMasterVolume)
            YandexGame.savesData.masterVolume = volume;
        else
            YandexGame.savesData.musicVolume = volume;
            
    }
}

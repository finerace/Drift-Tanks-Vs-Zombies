using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TrainingJhon : MonoBehaviour
{

    [SerializeField] private TMP_Text descLabel;
    [SerializeField] private Transform descLabelT;
    private Vector3 defaultLabelTScale;

    [Space] 
    
    [SerializeField] private int targetLvl;
    [SerializeField] private int targetTrainingLvl;
    [SerializeField] private int[] trainingTextIds;

    private int currentStage;

    [Space] 
    
    [SerializeField] private GameObject trainingPanel;

    private int currentLevel;
    private MenuSystem currentMenuSystem;
    
    private void Awake()
    {
        defaultLabelTScale = descLabelT.localScale;

        currentMenuSystem = GameObject.Find("GameMenu").GetComponent<MenuSystem>();
        
        LevelsLoadPassService.instance.OnLevelLoad += TrainingStart;
        void TrainingStart()
        {
            currentLevel = LevelsLoadPassService.instance.CurrentLevelData.Id;
            
            currentStage = -1;
            
            switch (currentLevel)
            {
                case 0:
                    YG.YandexGame.savesData.trainingStage = -1;
                    break;
            }
            
            if (LevelsLoadPassService.instance.CurrentLevelData.Id != targetLvl)
                return;
            
            if (YG.YandexGame.savesData.trainingStage >= targetTrainingLvl)
                return;
            
            GlobalGameEvents.instance.SetTrainingStartState(false);
            
            Time.timeScale = 0;
            Continue();
        }
    }

    public void Continue()
    {
        if (LevelsLoadPassService.instance.CurrentLevelData.Id != targetLvl)
            return;

        currentMenuSystem.isBackActionLock = true;
        
        currentStage++;
        if (currentStage >= trainingTextIds.Length)
        {
            Time.timeScale = 1;
            currentMenuSystem.isBackActionLock = false;
            
            trainingPanel.SetActive(false);
            YG.YandexGame.savesData.trainingStage = currentLevel;

            GlobalGameEvents.instance.SetTrainingStartState(true);

            return;
        }

        trainingPanel.SetActive(true);
        
        descLabel.text = CurrentLanguageData.GetText(trainingTextIds[currentStage]);

        descLabelT.localScale = defaultLabelTScale * 0.1f;
        descLabelT.DOScale(defaultLabelTScale, 0.25f);
        
        LevelsLoadPassService.instance.StartCoroutine(Still());
        IEnumerator Still()
        {
            Time.timeScale = 0;

            while (trainingPanel.activeSelf)
            {
                Time.timeScale = 0;
                yield return null;
            }
        }
    }

}

using System;
using UnityEngine;
using YG;

public class GameDataSaver : MonoBehaviour
{

    public static GameDataSaver instance;
    private SavesYG savesYg;

    public Action OnDataLoad;
    private bool isDataLoaded;
    public bool IsDataLoaded => isDataLoaded;

    private void Awake()
    {
        instance = this;
        
        if(!YandexGame.SDKEnabled)
            YandexGame.GetDataEvent += InitSaveData;
        else
            InitSaveData();
        
        void InitSaveData()
        {
            savesYg = YandexGame.savesData;
            isDataLoaded = true;
            OnDataLoad?.Invoke();
        }
    }

    private void Start()
    {
        //isDataLoaded = true;
        //OnDataLoad?.Invoke();
    }

    public void SetNewLeveHighScore(int levelId, int newHighScore)
    {
        savesYg.levelsData[levelId].levelHighScore = newHighScore;
    }

    public int GetLevelHighScore(int levelId)
    {
        return savesYg.levelsData[levelId].levelHighScore;
    }

    // public void SetLevelLockState(int levelId, bool state)
    // {
    //     if (savesYg.levelsData[levelId].isLevelUnlocked == state) 
    //         return;
    //     
    //     savesYg.levelsData[levelId].isLevelUnlocked = state;
    //
    //     Save();
    // }

    // public bool GetLockState(int levelId)
    // {
    //     return instance.savesYg.levelsData[levelId].isLevelUnlocked;
    // }

    public void SetLevelCompletedState(int levelId,bool state)
    {
        if (savesYg.levelsData[levelId].isLevelCompleted == state) 
            return;
        
        savesYg.levelsData[levelId].isLevelCompleted = state;
    }

    public bool IsLevelCompleted(int levelId)
    {
        return instance.savesYg.levelsData[levelId].isLevelCompleted;
    }

    public int GetGameScore()
    {
        if (!YandexGame.SDKEnabled)
            return 0;

        var resultScore = 0;
        
        foreach (var item in YandexGame.savesData.levelsData)
        {
            resultScore += item.levelHighScore;
        }

        return resultScore;
    }

    public void Save()
    {
        YandexGame.SaveProgress();
    }
    
}

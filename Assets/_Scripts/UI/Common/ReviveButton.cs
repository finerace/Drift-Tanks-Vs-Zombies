using System;
using UnityEngine;

public class ReviveButton : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private LevelsLoadPassService levelsLoadPassService;

    [Space] 
    
    [SerializeField] private int[] blockedLevels = new int[1];

    private void OnEnable()
    {
        var scores = LevelScoreCounter.instance;
        
        target.SetActive(scores.reviveCount <= LevelScoreCounter.maxReviveCount);
        
        UpdateStates();
    }

    private void UpdateStates()
    {
        foreach (var item in blockedLevels)
        {
            if (LevelsLoadPassService.instance.CurrentLevelData.Id == item)
            {
                target.SetActive(false);
                break;
            }
        }
    }
}

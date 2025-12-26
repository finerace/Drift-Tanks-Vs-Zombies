using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelObjectEnabler : MonoBehaviour
{
    private LevelsLoadPassService levelsLoadPassService;
    [SerializeField] private GameObject target;

    [Space] 
    
    [SerializeField] private int[] blockedLevels = new int[1];
    
    private void Start()
    {
        levelsLoadPassService = LevelsLoadPassService.instance;
        
        UpdateStates();
    }

    private void OnEnable()
    {
        UpdateStates();
    }

    private void UpdateStates()
    {
        foreach (var item in blockedLevels)
        {
            if (levelsLoadPassService.CurrentLevelData.Id == item)
            {
                target.SetActive(false);
                break;
            }
        }
        
        target.SetActive(true);
    }
    
}

using System;
using UnityEngine;

public class GlobalGameEvents : MonoBehaviour
{
    public static GlobalGameEvents instance;
    
    [SerializeField] private bool isLevelStarted;
    public event Action<bool> levelStartChangeState;
    
    public event Action<bool> trainingStartChangeState;
    
    private LevelData currentLevelData;
    public LevelData CurrentLevelData => currentLevelData;
    
    public void SetLevelStartState(bool state)
    {
        isLevelStarted = state;
        levelStartChangeState?.Invoke(isLevelStarted);
    }

    public void SetTrainingStartState(bool state)
    {
        trainingStartChangeState?.Invoke(state);
    }

    
    public void Awake()
    {
        instance = this;
    }
}

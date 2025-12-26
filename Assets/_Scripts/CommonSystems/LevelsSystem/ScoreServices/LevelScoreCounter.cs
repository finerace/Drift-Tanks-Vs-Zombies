using System;
using UnityEngine;

public class LevelScoreCounter : MonoBehaviour
{
    public static LevelScoreCounter instance;

    [SerializeField] private LevelsLoadPassService levelsLoadPassService;
    private TankEffects playerTankEffects;

    [Space]
    
    [SerializeField] private int earnedMoney;
    [SerializeField] private int earnedDonateMoney;
    
    [Space]
    
    [SerializeField] private int environmentDestructionScore = 0;
    [SerializeField] private int killedEnemiesScore = 0;
    [SerializeField] private float tankDriftScoreMultiplier = 1;

    [Space] 
    
    [SerializeField] private float driftScoreMultiplierPower = 0.025f;
    [SerializeField] private float levelCompleteTime;
    
    private LevelData currentLevelData;

    public float TankDriftScoreMultiplier => tankDriftScoreMultiplier;
    public float LevelCompleteTime => levelCompleteTime;
    public int EnvironmentDestructionScore => environmentDestructionScore;
    public int KilledEnemiesScore => killedEnemiesScore;

    public int EarnedMoney => earnedMoney;

    public int EarnedDonateMoney => earnedDonateMoney;

    public bool IsMoneyDuplicate;

    public int reviveCount;
    public const int maxReviveCount = 2;

    private void Awake()
    {
        instance = this;

        levelsLoadPassService.OnLevelLoad += SetPlayerTankEffects;
        void SetPlayerTankEffects()
        {
            playerTankEffects = FindObjectOfType<TankEffects>();
        }

        InitMoneyEarnCounter();
        void InitMoneyEarnCounter()
        {
            var playerMoney = FindObjectOfType<PlayerMoneyXpService>();

            playerMoney.OnMoneyChange += i => { earnedMoney += i;};
            playerMoney.OnDonateMoneyChange += i => { earnedDonateMoney += i;};
        }
    }

    private void Start()
    {
        LevelsLoadPassService.instance.OnPlayerRevive += () => { reviveCount++;};
    }

    private void Update()
    {
        DriftScoreMultiplierWork();
        void DriftScoreMultiplierWork()
        {
            if (playerTankEffects != null && playerTankEffects.IsTankDrifting)
            {
                tankDriftScoreMultiplier += Time.deltaTime * driftScoreMultiplierPower;
            }
        }

        if(currentLevelData != null)
            levelCompleteTime += Time.deltaTime;
    }

    public void AddEnvironmentDestructionScore(int score)
    {
        if (score <= 0)
            throw new ArgumentException();

        environmentDestructionScore += score;
    }

    public void AddEnemyKilledDestructionScore(int score)
    {
        if (score <= 0)
            throw new ArgumentException();

        killedEnemiesScore += score;
    }
    
    public int GetCompletedScore()
    {
        var finalScore =
            (environmentDestructionScore + killedEnemiesScore + GetScoreForLevelComplete()) * 
            (TankDriftScoreMultiplier + GetTimeScoreMultiplier() - 1f);

        return Mathf.RoundToInt(finalScore);
    }

    public int GetUncompletedScore()
    {
        var finalScore =
            (environmentDestructionScore + killedEnemiesScore) * tankDriftScoreMultiplier;

        return Mathf.RoundToInt(finalScore);
    }
    
    public float GetTimeScoreMultiplier()
    {
        var multiplier = currentLevelData.CompleteTime / levelCompleteTime;

        if (multiplier < 1)
            multiplier = 1;

        if (multiplier > 50)
            multiplier = 50;

        return multiplier;
    }

    public int GetScoreForLevelComplete()
    {
        return currentLevelData.CompleteScore;
    }
    
    public void ResetCounters()
    {
        earnedMoney = 0;
        earnedDonateMoney = 0;
        
        environmentDestructionScore = 0;
        killedEnemiesScore = 0;
        tankDriftScoreMultiplier = 1;
        levelCompleteTime = 0;

        IsMoneyDuplicate = false;
        reviveCount = 0;
    }

    public void SetNewCurrentLevelData(LevelData levelData)
    {
        currentLevelData = levelData;
    }

    public LevelData GetCurrentLevelData()
    {
        return currentLevelData;
    }

}

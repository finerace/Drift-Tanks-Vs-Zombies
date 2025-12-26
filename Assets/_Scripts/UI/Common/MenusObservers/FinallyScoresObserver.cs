using System.Collections;
using TMPro;
using UnityEngine;

public class FinallyScoresObserver : MonoBehaviour
{
    private LevelScoreCounter scoreCounter;

    [SerializeField] private float oneActAnimationTime;
    [SerializeField] private TextCounterAnimator textCounterAnimator;
    
    [Space]
    
    [SerializeField] private TMP_Text enemyDamageScore;
    [SerializeField] private TMP_Text environmentDamageScore;
    [SerializeField] private TMP_Text levelCompleteScore;
    
    [SerializeField] private TMP_Text driftMultiplier;
    
    [SerializeField] private TMP_Text completedTime;
    [SerializeField] private TMP_Text completedTimeMultiplier;

    [SerializeField] private TMP_Text finallyCalculates;
    [SerializeField] private TMP_Text finallyScore;

    [SerializeField] private TMP_Text earnedMoney;
    [SerializeField] private TMP_Text earnedDonateMoney;
    
    [Space] 
    
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    
    private Coroutine workCoroutine;
    
    private void Start()
    {
        scoreCounter = LevelScoreCounter.instance;
        LevelsLoadPassService.instance.OnScoreMultiplie += SetScoreMultiplier;
        
        void SetScoreMultiplier()
        {
            finallyScore.text = $"{(int)(scoreCounter.GetCompletedScore() * 1.1f)}";
        }
    }

    private IEnumerator ScoresAnimation()
    {
        var waiter = new WaitForSecondsRealtime(oneActAnimationTime + 0.05f);
        var miniWater = new WaitForSecondsRealtime(oneActAnimationTime /2);
        
        yield return waiter;
        
        textCounterAnimator.PlayAnimation
            (enemyDamageScore,0,scoreCounter.KilledEnemiesScore,oneActAnimationTime,AnimationType.integer);

        yield return waiter;
        
        textCounterAnimator.PlayAnimation
            (environmentDamageScore,
                0,scoreCounter.EnvironmentDestructionScore,oneActAnimationTime,AnimationType.integer);

        yield return waiter;

        textCounterAnimator.PlayAnimation
        (levelCompleteScore,
            0, scoreCounter.GetScoreForLevelComplete(), oneActAnimationTime, AnimationType.integer);
        
        yield return waiter;

        textCounterAnimator.PlayAnimation
            (driftMultiplier,
                1f,scoreCounter.TankDriftScoreMultiplier,oneActAnimationTime,AnimationType.floating);

        yield return waiter;

        completedTime.text = scoreCounter.LevelCompleteTime.ConvertSecondsToTimer();
        
        textCounterAnimator.PlayAnimation
            (completedTimeMultiplier,1f,
                scoreCounter.GetTimeScoreMultiplier(),oneActAnimationTime,AnimationType.floating);

        yield return waiter;

        finallyCalculates.text =
            $"{scoreCounter.KilledEnemiesScore + scoreCounter.EnvironmentDestructionScore + scoreCounter.GetScoreForLevelComplete()} * " +
            $"x{(scoreCounter.TankDriftScoreMultiplier + scoreCounter.GetTimeScoreMultiplier() - 1f).ConvertToString()} =";
        
        yield return miniWater;

        var completedScore = scoreCounter.GetCompletedScore();
        
        textCounterAnimator.PlayAnimation
            (finallyScore,0,scoreCounter.GetCompletedScore(),oneActAnimationTime,AnimationType.integer);
        
        yield return waiter;
        
        textCounterAnimator.PlayAnimation
            (earnedMoney,0,scoreCounter.EarnedMoney,oneActAnimationTime,AnimationType.integer);

        if(completedScore > scoreCounter.GetCurrentLevelData().OneStarScore)
            star1.SetActive(true);
        
        yield return miniWater;

        if(completedScore > scoreCounter.GetCurrentLevelData().TwoStarScore)
            star2.SetActive(true);
        
        yield return miniWater;

        textCounterAnimator.PlayAnimation
            (earnedDonateMoney,0,scoreCounter.EarnedDonateMoney,oneActAnimationTime,AnimationType.integer);

        if(completedScore > scoreCounter.GetCurrentLevelData().ThreeStarScore)
            star3.SetActive(true);
    }

    public void ResetValues()
    {
        enemyDamageScore.text = "0";
        environmentDamageScore.text = "0";
        levelCompleteScore.text = "0";

        driftMultiplier.text = "x1.00";

        completedTime.text = "00:00:00";
        completedTimeMultiplier.text = "x1.00";

        finallyCalculates.text = "--- * --- =";
        finallyScore.text = "0";

        earnedMoney.text = "0";
        earnedDonateMoney.text = "0";
        
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        
        if(workCoroutine != null)
            StopCoroutine(workCoroutine);
    }

    private void StartAnimation()
    {
        workCoroutine = StartCoroutine(ScoresAnimation());
    }

    private void OnEnable()
    {
        ResetValues();
        StartAnimation();
    }
}

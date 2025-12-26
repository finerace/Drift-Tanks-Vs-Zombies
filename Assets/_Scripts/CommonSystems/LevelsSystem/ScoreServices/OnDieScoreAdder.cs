using System;
using UnityEngine;

public class OnDieScoreAdder : MonoBehaviour
{
    [SerializeField] private HealthBase target;
    [SerializeField] private ScoreType scoreType;
    [SerializeField] private int scoreValue;

    private void Start()
    {
        return;
        
        target.OnDie += AddScore;

        void AddScore()
        {
            var levelScores = LevelScoreCounter.instance;

            switch (scoreType)
            {
                case ScoreType.Environment:
                    levelScores.AddEnvironmentDestructionScore(scoreValue);
                    break;
                case ScoreType.Enemies:
                    levelScores.AddEnemyKilledDestructionScore(scoreValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private enum ScoreType
    {
        Environment,
        Enemies
    }
}

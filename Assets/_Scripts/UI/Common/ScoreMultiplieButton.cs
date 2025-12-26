using System;
using UnityEngine;

public class ScoreMultiplieButton : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    private void Start()
    {
        LevelsLoadPassService.instance.OnScoreMultiplie += () => target.SetActive(false);
    }

    private void OnEnable()
    {
        target.SetActive(true);
    }
}

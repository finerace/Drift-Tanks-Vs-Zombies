using System;
using System.Collections;
using UnityEngine;

public class MoneyDuplicateButt : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    private void Start()
    {
        PlayerMoneyXpService.instance.OnMoneyDuplicate += Disable;
        void Disable()
        {
            target.SetActive(false);
        }
    }

    private void OnEnable()
    {
        var scores = LevelScoreCounter.instance;

        StartCoroutine(Still());
        IEnumerator Still()
        {
            yield return null;

            if(scores != null && scores.IsMoneyDuplicate)
            {
                target.SetActive(false);
                yield break;
            }
        
            target.SetActive(scores != null && 
                             !scores.IsMoneyDuplicate && 
                             scores.EarnedMoney > 0 || 
                             scores.EarnedDonateMoney > 0);
        }
    }
}

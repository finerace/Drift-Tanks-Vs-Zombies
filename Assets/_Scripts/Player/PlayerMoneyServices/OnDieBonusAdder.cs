using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OnDieBonusAdder : MonoBehaviour
{
    [SerializeField] private HealthBase targetHealth;
    private PlayerMoneyXpService playerMoneyXpService;
    
    private const int maxChance = 1000;
    
    [Space] 
    [SerializeField] [Range(0,maxChance)] private int moneyAddChance = 0;
    [SerializeField] private int moneyAdd;
    
    [SerializeField] [Range(0,maxChance)] private int donateMoneyAddChance = 0;
    [SerializeField] private int donateMoneyAdd;
    
    [SerializeField] [Range(0,maxChance)] private int xpAddChance = 0;
    [SerializeField] private int XpAdd;

    private void Start()
    {
        return;
        
        playerMoneyXpService = PlayerMoneyXpService.instance;
        
        if(targetHealth != null)
            targetHealth.OnDie += AddBonus;

        void AddBonus()
        {
            if (moneyAdd > 0 && Random.Range(0, maxChance) <= moneyAddChance)
                playerMoneyXpService.PlayerMoney += moneyAdd;
            
            if (donateMoneyAdd > 0 && Random.Range(0, maxChance) <= donateMoneyAddChance)
                playerMoneyXpService.PlayerDonateMoney += donateMoneyAdd;

            if (XpAdd > 0 && Random.Range(0, maxChance) <= xpAddChance)
                playerMoneyXpService.PlayerXp += XpAdd;
        }
    }
}

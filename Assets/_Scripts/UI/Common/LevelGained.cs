using UnityEngine;

public class LevelGained : MonoBehaviour
{
    [SerializeField] private GameObject[] targets;
    
    private void Start()
    {
        PlayerMoneyXpService.instance.OnLevelGained += CheckLevelGained;
    }

    private void OnEnable()
    {
        CheckLevelGained();
    }

    private void CheckLevelGained()
    {
        var instance = PlayerMoneyXpService.instance;
        
        if(instance == null)
            return;
        
        foreach (var VARIABLE in targets)
        {
            VARIABLE.SetActive(PlayerMoneyXpService.instance.IsLevelRewardNotGained());   
        }
    }

}

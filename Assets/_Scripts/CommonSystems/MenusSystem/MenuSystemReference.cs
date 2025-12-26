using System;
using UnityEngine;
using YG;

public class MenuSystemReference : MonoBehaviour
{
    private MenuSystem menuSystem;
    public MenuSystem MenuSystem => menuSystem;

    public void Start()
    {
        if(menuSystem == null)
            menuSystem = FindObjectOfType<MenuSystem>();
    }

    public void SetMenuSystemReference(MenuSystem menuSystemRef)
    {
        menuSystem = menuSystemRef;
    }

    public void MenuSystemBack()
    {
        menuSystem.Back();
    }

    public void MenuSystemOpenMenu(string menuId)
    {
        menuSystem.OpenLocalMenu(menuId);
    }
    
    public void MenuSystemOpenTotal(string menuId)
    {
        menuSystem.ActivateMenu(menuId);
    }

    public void ContinueLevel()
    {
        LevelsLoadPassService.instance.Continue();
    }

    public void BuyInShop(string itemId)
    {
        YandexGame.BuyPayments(itemId);
    }

    public void GainLevel()
    {
        PlayerMoneyXpService.instance.GetLevelReward();
    }

    public void ShowRewardedAd(int id)
    {
        YandexGame.RewVideoShow(id);
    }
    
}

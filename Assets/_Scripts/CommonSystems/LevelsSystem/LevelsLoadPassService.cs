using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using YG;

public class LevelsLoadPassService : MonoBehaviour
{
    public static LevelsLoadPassService instance;

    [SerializeField] private LevelData startLevel;
    [SerializeField] private GlobalGameEvents globalGameEvents;
    
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private LevelScoreCounter levelScoreCounter;
    
    [SerializeField] private Transform levelSpawnPoint;
    [SerializeField] private Transform playerTankT;

    [Space] 
    
    [SerializeField] private Transform menuRoom;
    [SerializeField] private Transform mainCameraT;
    [SerializeField] private Transform inMenuCameraPoint;
    
    [Space] 
    
    [SerializeField] private MenuSystem mainMenu;
    [SerializeField] private MenuSystem gameMenu;
    [SerializeField] private GameObject loadMenu;
    
    [Space]
    
    [SerializeField] private LevelData currentLevelData;
    private GameObject currentLevel;
    
    private bool isCurrentLevelComplete = false;
    private Coroutine dieCoroutine;

    private GameDataSaver gameDataSaver;

    public LevelData CurrentLevelData => currentLevelData;

    public event Action OnScoreMultiplie;
    public event Action OnLevelLoad;

    public event Action OnPlayerRevive;

    private int adCount;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameDataSaver = GameDataSaver.instance;

        if (!YandexGame.SDKEnabled)
        {
            YandexGame.GetDataEvent += () => {if(YandexGame.savesData.trainingStage <= -2)
                LoadLevel(startLevel);};
        }
        else if(YandexGame.savesData.trainingStage <= -2)
            LoadLevel(startLevel);
    }

    public void LoadLevel(LevelData levelData)
    {
        gameMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        loadMenu.SetActive(true);

        StartCoroutine(LOAD());
        IEnumerator LOAD()
        {
            yield return null;
            yield return null;
            yield return null;
            
            if (currentLevel != null)
                Destroy(currentLevel);

            SpawnPlayer();

            void SpawnPlayer()
            {
                var currentTankAllData = TanksShopService.instance.GetCurrentTankData();

                playerTankT =
                    Instantiate(currentTankAllData.tankShopData.Tank, levelSpawnPoint.position,
                        levelSpawnPoint.rotation).transform;

                playerTankT.gameObject.GetComponent<PlayerTank>()
                    .SetTankCharacteristics(currentTankAllData.tankShopData, currentTankAllData.tankSaveData);

                if (!playerTankT.gameObject.activeSelf)
                    playerTankT.gameObject.SetActive(true);
            }

            currentLevelData = levelData;
            currentLevel = Instantiate(currentLevelData.LevelPrefab);
            
            if(currentLevel.TryGetComponent<LevelGenerator>(out LevelGenerator gen))
                gen.GenerateLevel();

            menuRoom.gameObject.SetActive(false);
            virtualCamera.gameObject.SetActive(true);
            virtualCamera.Follow = playerTankT;
            virtualCamera.LookAt = playerTankT;

            levelScoreCounter.ResetCounters();
            levelScoreCounter.SetNewCurrentLevelData(currentLevelData);

            ChangeMenusActivity(false);

            globalGameEvents.SetLevelStartState(true);

            SetPlayerDieAlgorithm();

            void SetPlayerDieAlgorithm()
            {
                playerTankT.gameObject.GetComponent<PlayerTank>().OnDie += () =>
                {
                    dieCoroutine = StartCoroutine(OnPlayerDie());
                };

                IEnumerator OnPlayerDie()
                {
                    yield return new WaitForSeconds(3);

                    gameMenu.isBackActionLock = true;
                    StopLevel();
                }
            }

            loadMenu.SetActive(false);
            
            OnLevelLoad?.Invoke();
        }

        adCount++;

        if (adCount >= 2)
        {
            adCount = 0;
            
            if(YandexGame.SDKEnabled)
                YandexGame.FullscreenShow();
        }

    }

    public void StopLevel()
    {
        if (currentLevelData == null)
            throw new Exception("Currently level is null!");
        
        SaveHighScore();
        void SaveHighScore()
        {
            var oldHighScore = gameDataSaver.GetLevelHighScore(currentLevelData.Id);
            var currentScore = levelScoreCounter.GetUncompletedScore(); 
            
            if(currentScore > oldHighScore)
                gameDataSaver.SetNewLeveHighScore(currentLevelData.Id,currentScore);
        }
        
        gameDataSaver.Save();

        gameMenu.OpenLocalMenu("StopLevel");
    }

    public void RestartLevel()
    {
        if (currentLevelData == null)
            throw new Exception("Currently level is null!");
        
        var savedLevelData = currentLevelData;
        
        UnloadLevel();
        LoadLevel(savedLevelData);
    }
    
    public void UnloadLevel()
    {
        gameDataSaver.Save();

        currentLevelData = null;
        if(currentLevel != null)
            Destroy(currentLevel);

        isCurrentLevelComplete = false;
        Destroy(playerTankT.gameObject);

        ChangeMenusActivity(true);
        gameMenu.isBackActionLock = false;

        menuRoom.gameObject.SetActive(true);
        virtualCamera.gameObject.SetActive(false);
        mainCameraT.SetPositionAndRotation(inMenuCameraPoint.position,inMenuCameraPoint.rotation);
        
        DisableDieCoroutine();
        
        levelScoreCounter.SetNewCurrentLevelData(null);
        
        globalGameEvents.SetLevelStartState(false);
    }
    
    public void CompleteLevel()
    {
        if(isCurrentLevelComplete)
            return;

        gameMenu.isBackActionLock = true;
        isCurrentLevelComplete = true;
        
        if (currentLevelData == null)
            throw new Exception("Currently level is null!");
        
        gameMenu.OpenLocalMenu("LevelComplete");
        
        SetNewResults();
        void SetNewResults()
        {
            NewHighScoreWork();
            void NewHighScoreWork()
            {
                var oldHighScore = gameDataSaver.GetLevelHighScore(currentLevelData.Id);
                var currentScore = levelScoreCounter.GetCompletedScore(); 
            
                if(currentScore > oldHighScore)
                    gameDataSaver.SetNewLeveHighScore(currentLevelData.Id,currentScore);
            }

            var isLevelCompleted = gameDataSaver.IsLevelCompleted(currentLevelData.Id);

            AddForLevelCompleteMoney();
            void AddForLevelCompleteMoney()
            {
                var playerMoney = PlayerMoneyXpService.instance;
                
                if (!isLevelCompleted)
                {
                    playerMoney.PlayerMoney += currentLevelData.CompleteMoney;
                    playerMoney.PlayerDonateMoney += currentLevelData.CompleteDonateMoney;
                    playerMoney.PlayerXp += currentLevelData.CompleteXp;
                }
                else
                {
                    playerMoney.PlayerMoney += currentLevelData.CompleteMoney / 10;
                    playerMoney.PlayerXp += currentLevelData.CompleteXp / 10;
                }
            }
            
            if (!isLevelCompleted)
                gameDataSaver.SetLevelCompletedState(currentLevelData.Id, true);

            if (YandexGame.initializedLB && YandexGame.auth)
            {
                var currentGameScore = gameDataSaver.GetGameScore();

                if (YandexGame.savesData.highestGameScore < currentGameScore)
                {
                    YandexGame.savesData.highestGameScore = currentGameScore;
                    
                    YandexGame.NewLeaderboardScores("HighestScore",currentGameScore);
                }
                
            }
                
        }
        
        gameDataSaver.Save();
        
        DisableDieCoroutine();
    }

    public void Continue()
    {
        var nextLevelData = LevelDataSTATIC.GetLevelData(currentLevelData.Id);
        
        UnloadLevel();
        LoadLevel(nextLevelData);
    }

    public void RevivePlayer()
    {
        playerTankT.GetComponent<PlayerTank>().Reset();
        var menu = FindObjectOfType<MenuSystem>();

        menu.isBackActionLock = false;
        menu.Back();
        
        DisableDieCoroutine();

        StartCoroutine(TimeFix());
        IEnumerator TimeFix()
        {
            yield return null;
            Time.timeScale = 1;
        }
        
        OnPlayerRevive?.Invoke();
    }

    public void ScoreMultiplier()
    {
        var oldHighScore = gameDataSaver.GetLevelHighScore(currentLevelData.Id);
        var currentScore = levelScoreCounter.GetCompletedScore() * 1.1f; 
            
        if(currentScore > oldHighScore)
            gameDataSaver.SetNewLeveHighScore(currentLevelData.Id,(int)currentScore);
        
        OnScoreMultiplie?.Invoke();
    }
    
    private void DisableDieCoroutine()
    {
        if (dieCoroutine != null)
            StopCoroutine(dieCoroutine);
    }

    private void ChangeMenusActivity(bool isMainMenuActive)
    {
        mainMenu.gameObject.SetActive(isMainMenuActive);
        if(isMainMenuActive)
            mainMenu.OpenStartMenu();
        
        gameMenu.gameObject.SetActive(!isMainMenuActive);
        if(!isMainMenuActive)
            gameMenu.OpenStartMenu();
    }
    
    /*public void SetNewLevelData(LevelData levelData)
    {
        if (currentLevelData == null)
            currentLevelData = levelData;
        else throw new FieldAccessException("Current level is not stopped! New level data not be approved!");
    }*/
}

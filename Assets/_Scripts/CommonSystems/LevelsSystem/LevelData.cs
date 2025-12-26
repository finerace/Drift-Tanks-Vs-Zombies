using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private int name;
    [SerializeField] private GameObject levelPrefab;
    
    [Space]
    
    [SerializeField] private float completeTime;
    [SerializeField] private int completeScore;
    
    [SerializeField] private int completeMoney;
    [SerializeField] private int completeDonateMoney;
    [SerializeField] private int completeXp;
    
    [Space] 
    
    [SerializeField] private int oneStarScore;
    [SerializeField] private int twoStarScore;
    [SerializeField] private int threeStarScore;
    
    public int Id => id;
    public int Name => name;
    public GameObject LevelPrefab => levelPrefab;
    public float CompleteTime => completeTime;
    public int CompleteScore => completeScore;
    public int CompleteMoney => completeMoney;
    public int CompleteDonateMoney => completeDonateMoney;
    public int CompleteXp => completeXp;

    public int OneStarScore => oneStarScore;
    public int TwoStarScore => twoStarScore;
    public int ThreeStarScore => threeStarScore;
}

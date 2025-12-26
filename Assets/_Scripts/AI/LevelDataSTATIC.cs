using System;
using UnityEngine;

public class LevelDataSTATIC : MonoBehaviour
{
    [SerializeField] private LevelData[] levelDatas = new LevelData[30];
    private static LevelData[] levelDatasS;

    private void Awake()
    {
        levelDatasS = levelDatas;
    }

    public static LevelData GetLevelData(int id)
    {
        return id+1 >= levelDatasS.Length ? levelDatasS[0] : levelDatasS[id+1];
    }
}

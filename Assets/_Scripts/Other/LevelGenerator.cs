using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private RoadsPrefs roads;
    [SerializeField] private int levelScale = 18;
    
    [Space] 
    
    [SerializeField] private int newPointGenerateAllowChance = 30;
    [SerializeField] private int pointRotateChance = 40;

    [Space] 
    
    [SerializeField] private int newPointGenerateChance = 30;

    [SerializeField] private int startPointsGenerateChance = 75;

    [Space] 
    
    [SerializeField] private int additionLevelSize = 4;
    [SerializeField] private int cityDepth = 2;
    [SerializeField] private int bordersDepth = 2;

    [Space] 
    
    [SerializeField] private int maxZombiesPerRoad = 6;
    [SerializeField] private int zombieSpawnChance = 35;
    
    [SerializeField] private int maxZombiesGPerRoad = 0;
    [SerializeField] private int zombieGSpawnChance = 0;
    
    [SerializeField] private bool generateNewSeed = false;
    [SerializeField] private int seed;
    
    private (int,int)[,] levelMap;
    
    [ContextMenu("Generate Level")]
    public void GenerateLevel()
    {
        ClearChilds();
        void ClearChilds()
        {
            if(generateNewSeed)
                transform.DeleteChilds();
        }

        levelMap = new(int,int)[levelScale, levelScale];

        var maxSteps = levelScale * levelScale * 8;
        var currentSteps = 0;
        const int rotationMultiplier = 90;
        const int cellScale = 15;
        
        var generationPoints = new List<Transform>(1024);

        if(generateNewSeed)
            seed = UnityEngine.Random.Range(-9999999,9999999);
        
        var randomizer = new Random(seed);
        roads.randomizer = randomizer;
        
        if (levelScale < 4)
            throw new Exception("Level is too small!");

        (int, int) GetLevelCell(Transform point)
        {
            var x = Mathf.RoundToInt(point.position.x / cellScale);
            var z = Mathf.RoundToInt(point.position.z / cellScale);

            if ((x < 0 || x >= levelScale) || (z < 0 || z >= levelScale))
                return (-9, -9);
            
            return levelMap[z, x];
        }
        (int, int) GetLevelCellNext(Vector3 pointPos,Vector3 direction,bool isDat = false)
        {
            var x = Mathf.RoundToInt((pointPos.x + direction.x * cellScale) / cellScale);
            var z = Mathf.RoundToInt((pointPos.z + direction.z * cellScale) / cellScale);

            if ((x < 0 || x >= levelScale) || (z < 0 || z >= levelScale))
                return (-9, -9);

            if (!isDat)
                return levelMap[x, z];
            else
                return levelMap[z, x];
        }
        void SetLevelCell(Transform generationPoint,(int,int) value)
        {
            levelMap[Mathf.RoundToInt(generationPoint.position.z / cellScale), 
                    Mathf.RoundToInt(generationPoint.position.x / cellScale)]
                = value;
        }
        Vector2 GetCellEnvironmentCof(Vector3 cellPos,Transform parent = null,bool isDat = false)
        {
            var finalCof = new Vector2();
            var checkBool = false;

            var forward = Vector3.forward;
            var right = Vector3.right;

            if (parent != null)
            {
                forward = parent.forward;
                right = parent.right;
            }

            bool IsCellNotEmpty(int cellId)
            {
                return cellId != 0 && cellId != 9;
            }
            if (IsCellNotEmpty(GetLevelCellNext(cellPos, forward,isDat).Item1))
            {
                checkBool = true;
                finalCof.y++;
            }

            if (IsCellNotEmpty(GetLevelCellNext(cellPos, -forward,isDat).Item1))
            {
                checkBool = true;
                finalCof.y--;
            }

            if (!checkBool)
                finalCof.y = -9;
            checkBool = false;
            
            if (IsCellNotEmpty(GetLevelCellNext(cellPos, -right,isDat).Item1))
            {
                checkBool = true;
                finalCof.x--;
            }

            if (IsCellNotEmpty(GetLevelCellNext(cellPos, right,isDat).Item1))
            {
                checkBool = true;
                finalCof.x++;
            }
            
            if (!checkBool)
                finalCof.x = -9;
            
            return finalCof;
        }
        bool RandChance(int chance)
        {
            var rand = randomizer.Next(0, 100);

            return chance >= rand;
        }
        
        GenerateRoads();
        void GenerateRoads()
        {
            Transform CreatePoint(Transform parent,int rotationId)
            {
                var newGenerationPoint = new GameObject().transform;

                newGenerationPoint.position = parent.position;
                newGenerationPoint.eulerAngles = parent.eulerAngles + new Vector3(0,rotationId * rotationMultiplier,0);
                generationPoints.Add(newGenerationPoint);
                
                return newGenerationPoint;
            }

            void GoForward(Transform point)
            {
                point.position += point.forward * cellScale;
            }
            void SetRotation(Transform point,int rotationId,Transform parent = null)
            {
                if(parent == null)
                    point.eulerAngles = new Vector3(0, rotationId * rotationMultiplier, 0);
                else
                    point.eulerAngles = parent.eulerAngles + new Vector3(0, rotationId * rotationMultiplier, 0);
            }
            bool IsPointLifeAllow(Transform parent, int rotationId,bool isForNewPoints = false)
            {
                var cof = GetCellEnvironmentCof(parent.position,parent,true);
                var result = cof.x < -2;

                if (!isForNewPoints) 
                    return result;
                
                cof = GetCellEnvironmentCof(parent.position + parent.forward * cellScale,parent,true);
                var cof2 = GetCellEnvironmentCof(parent.position - parent.forward * cellScale,parent,true);

                result = ((cof.x < -2 || cof.x != rotationId) && cof.x != 0) &&
                         ((cof2.x < -2 || cof2.x != rotationId) && cof2.x != 0);

                return result;
            }
           
            //Test();
            //return;
            void Test()
            {
                levelMap[17, 15] = (-1, 0);
                levelMap[15, 17] = (-1, 0);

                var point = 
                    new GameObject().transform;
                point.position = new Vector3(16 * cellScale, 0, 16 * cellScale);
            
                point.Rotate(new Vector3(0,0,0));
            
                var data = GetCellEnvironmentCof(point.position,point);
                //print(data.x);
                print(IsPointLifeAllow(point,-1,true));
                DestroyImmediate(point.gameObject);
            }
            
            levelMap[levelScale / 2, levelScale / 2] = (-1, 0);

            var firstPoint = CreateFirstPoint();
            Transform CreateFirstPoint()
            {
                var firstPoint = new GameObject().transform;
                firstPoint.position = new Vector3(levelScale / 2 * 15, 0, levelScale / 2 * 15);
                firstPoint.eulerAngles = new Vector3(0, randomizer.Next(0, 4) * rotationMultiplier, 0);
                
                generationPoints.Add(firstPoint);
                return firstPoint;
            }
            
            RotateParentPointAndGenerateNewPoints(firstPoint,true);
            void RotateParentPointAndGenerateNewPoints(Transform parent,bool isFirst = false)
            {
                if (!isFirst)
                {
                    if (IsPointLifeAllow(parent,-1,true) && RandChance(newPointGenerateChance))
                        CreatePoint(parent, -1);
                    
                    if (IsPointLifeAllow(parent,1,true) && RandChance(newPointGenerateChance))
                        CreatePoint(parent, 1);
                }
                else
                {
                    if (RandChance(startPointsGenerateChance))
                        CreatePoint(parent, -1);
                    
                    if (RandChance(startPointsGenerateChance))
                        CreatePoint(parent, 1);
                    
                    if (RandChance(startPointsGenerateChance))
                        CreatePoint(parent, 2);
                }
            }
            
            PointsMoving();
            void PointsMoving()
            {
                while (currentSteps <= maxSteps)
                {
                    currentSteps++;
                    for (int i = 0; i < generationPoints.Count; i++)
                    {
                        var point = generationPoints[i];

                        void PointDelete()
                        {
                            generationPoints.Remove(point);
                            DestroyImmediate(point.gameObject);
                        }
                        
                        GoForward(point);

                        var currentLevelCell = GetLevelCell(point);
                        if (currentLevelCell.Item1 == 0)
                        {
                            SetLevelCell(point,(-1,currentLevelCell.Item2));
                            
                            if (!IsPointLifeAllow(point,0))
                            {
                                PointDelete();
                                continue;
                            }
                        }
                        else
                        {
                            PointDelete();
                            continue;
                        }

                        if (RandChance(pointRotateChance))
                            SetRotation(point,randomizer.Next(-1,2),point);
                        else if(RandChance(newPointGenerateAllowChance))
                            RotateParentPointAndGenerateNewPoints(point);
                    }
                }
                
                //print(d + " d");
                //print(currentSteps);
            }
            
        }
        
        SetRoads();
        void SetRoads()
        {
            // levelMap[levelScale / 2, levelScale / 2] = (-1, 0);
            // levelMap[levelScale / 2-1, levelScale / 2] = (-1, 0);
            // levelMap[levelScale / 2+1, levelScale / 2] = (-1, 0);
            // levelMap[levelScale / 2-1, levelScale / 2+1] = (-1, 0);
            // levelMap[levelScale / 2, levelScale / 2+1] = (-1, 0);
            //
            // print(GetCellEnvironmentCof(
            //     new Vector3((levelScale / 2)*cellScale,0, (levelScale / 2+1)*cellScale)));
            
            for (int i = 0; i < levelScale; i++)
            {
                for (int j = 0; j < levelScale; j++)
                {
                    if(levelMap[i,j] == (0,0))
                        continue;
                        
                    var cellEnvironmentVector = GetCellEnvironmentCof(new Vector3(i*cellScale,0,j*cellScale));
                    var cellEnvironment = (cellEnvironmentVector.x, cellEnvironmentVector.y);

                    var result = (0, 0);
                    
                    switch (cellEnvironment)
                    {
                        case (0,0):
                        {
                            result = (3, 0);
                            break;
                        }
                        
                        case (1,0):
                        {
                            result = (2, -1);
                            break;
                        }

                        case (-1,0):
                        {
                            result = (2, 1);
                            break;
                        }

                        case (0,1):
                        {
                            result = (2, 2);
                            break;
                        }

                        case (0,-1):
                        {
                            result = (2, 0);
                            break;
                        }

                        case (1,-1):
                        {
                            result = (4, -1);
                            break;
                        }

                        case (1,1):
                        {
                            result = (4, 2);
                            break;
                        }
                        
                        case (-1,1):
                        {
                            result = (4, 1);
                            break;
                        }
                        
                        case (-1,-1):
                        {
                            result = (4, 0);
                            break;
                        }
                        
                        case (-9,0):
                        {
                            result = (1, 0);
                            break;
                        }
                        
                        case (0,-9):
                        {
                            result = (1, 1);
                            break;
                        }
                        
                        case (1,-9):
                        {
                            result = (1, 1);
                            break;
                        }
                        
                        case (-1,-9):
                        {
                            result = (1, 1);
                            break;
                        }
                        
                        case (-9,1):
                        {
                            result = (1, 0);
                            break;
                        }
                        
                        case (-9,-1):
                        {
                            result = (1, 0);
                            break;
                        }
                        
                        case (-9,-9):
                        {
                            result = (0, 0);
                            break;
                        }
                        
                        default:
                        {
                            result = (3, 0);
                            break;
                        }
                    }

                    levelMap[i, j] = result;
                }   
            }
        }

        bool IsIdOffLevel((int,int) id)
        {
            return id.Item1 < 0 || id.Item1 >= levelScale || id.Item2 < 0 || id.Item2 >= levelScale;
        }
        
        int GetShortestRoadDistance((int, int) id)
        {
            bool IsCellRoad((int,int) id)
                {
                    if (IsIdOffLevel(id))
                        return false;
                        
                    var cellId = levelMap[id.Item1, id.Item2].Item1;

                    return cellId is >= 1 and <= 4;
                }

            var maxDistance = cityDepth + bordersDepth + 1;
                
            for (int i = 1; i <= maxDistance+1; i++)
            {
                for (int j = 1; j <= i+2; j++)
                {
                    var direction = new Vector2Int(1, 0);
                    var cx = id.Item1 - i;
                    var cy = id.Item2 + i;

                    (int,int) CheckIdCalculate()
                    {
                        return (cx + direction.x * j, cy + direction.y * j);
                    }
                    
                    var checkingId = CheckIdCalculate();
                    
                    if (IsCellRoad(checkingId))
                        return i-1;
                        
                        
                    direction = new Vector2Int(0, -1);
                    cx = id.Item1 + i;
                    cy = id.Item2 + i;
                        
                    checkingId = CheckIdCalculate();
                    
                    if (IsCellRoad(checkingId))
                        return i-1;
                        
                        
                    direction = new Vector2Int(-1, 0);
                    cx = id.Item1 + i;
                    cy = id.Item2 - i;
                        
                    checkingId = CheckIdCalculate();
                    
                    if (IsCellRoad(checkingId))
                        return i-1;
                        
                        
                    direction = new Vector2Int(0, 1);
                    cx = id.Item1 - i;
                    cy = id.Item2 - i;
                        
                    checkingId = CheckIdCalculate();
                    
                    if (IsCellRoad(checkingId))
                        return i-1;
                }
            }

            return maxDistance;
        }
        
        var offLevelMap = new(int,int)[levelScale + additionLevelSize*2, levelScale + additionLevelSize*2];
        
        SetCity();
        void SetCity()
        {
            for (int i = -additionLevelSize; i < levelScale + additionLevelSize; i++)
            {
                for (int j = -additionLevelSize; j < levelScale + additionLevelSize; j++)
                {
                    
                    var currentId = (i, j);
                    var toRoad = 0;

                    if (IsIdOffLevel(currentId))
                    {
                        toRoad = GetShortestRoadDistance(currentId);

                        var trueI = i + additionLevelSize;
                        var trueJ = j + additionLevelSize;
                        
                        if (toRoad <= cityDepth)
                            offLevelMap[trueI, trueJ] = (5, 0);
                        else if (toRoad < cityDepth+bordersDepth)
                            offLevelMap[trueI, trueJ] = (6, 0);
                        
                        continue;
                    }
                    
                    if(levelMap[i,j] != (0,0))
                        continue;
                    
                    toRoad = GetShortestRoadDistance(currentId);

                    if (toRoad <= cityDepth)
                        levelMap[i, j] = (5, 0);
                    else if (toRoad <= cityDepth+bordersDepth)
                        levelMap[i, j] = (6, 0);
                }   
            }
        }
        
        FinalSpawn();
        void FinalSpawn()
        {
            var points = generationPoints.ToArray();
            
            for (int i = 0; i < points.Length; i++)
            {
                var item = points[i];
                if(item != null)
                    DestroyImmediate(generationPoints[i].gameObject);
            }

            var parentT = new GameObject().transform;
            
            for (int i = -additionLevelSize; i < levelScale + additionLevelSize; i++)
            {
                for (int j = -additionLevelSize; j < levelScale + additionLevelSize; j++)
                {
                    if (IsIdOffLevel((j, i)))
                    {
                        var trueJ = j + additionLevelSize;
                        var trueI = i + additionLevelSize;
                        
                        var offCellData = offLevelMap[trueJ,trueI];
                        if(offCellData == (0,0))
                            continue;

                        var miniCellScale = cellScale / 4f;

                        void SpawnMiniCell(int x,int y)
                        {
                            parentT.position = new Vector3
                            (j * cellScale - (levelScale) / 2 * cellScale + miniCellScale * x, 0, 
                                i * cellScale - (levelScale) / 2 * cellScale + miniCellScale * y);

                            parentT.rotation =
                                Quaternion.Euler(new Vector3(0, rotationMultiplier * randomizer.Next(0, 4), 0));
                            
                            var spawnedCityPart = Instantiate(roads.GetRoad
                                (offCellData.Item1 - 1),parentT);

                            spawnedCityPart.transform.parent = transform;
                        }
                        
                        SpawnMiniCell(1,1);
                        SpawnMiniCell(1,-1);
                        SpawnMiniCell(-1,-1);
                        SpawnMiniCell(-1,1);
                        
                        continue;
                    }
                    
                    var cellData = levelMap[j, i];
                    if (cellData.Item1 > 0 && cellData.Item1 <= 4)
                    {
                        var cellCenter = new Vector3
                        (j * cellScale - levelScale / 2 * cellScale, 0,
                            i * cellScale - levelScale / 2 * cellScale);
                        
                        SpawnRoad();
                        void SpawnRoad()
                        {
                            parentT.position = cellCenter;

                            parentT.rotation =
                                Quaternion.Euler(new Vector3(0, rotationMultiplier * cellData.Item2, 0));

                            var spawnedCityPart = Instantiate(roads.GetRoad
                                (cellData.Item1 - 1), parentT);

                            spawnedCityPart.transform.parent = transform;
                        }

                        SpawnZombies();
                        void SpawnZombies()
                        {
                            for (int k = 0; k < maxZombiesPerRoad; k++)
                            {
                                if(!RandChance(zombieSpawnChance))
                                    continue;

                                var halfCellScale = cellScale / 2f;
                                
                                var spawnPos = 
                                    new Vector3(randomizer.Next((int)-halfCellScale, (int)halfCellScale),0,
                                        randomizer.Next((int)-halfCellScale, (int)halfCellScale));

                                var spawnRotation = Quaternion.Euler(new Vector3(0,randomizer.Next(0,360),0));

                                parentT.position = cellCenter + spawnPos;
                                parentT.rotation = spawnRotation;
                                
                                var zombie = Instantiate(roads.zombie, parentT);
                                zombie.transform.parent = transform;
                            }
                            
                            for (int k = 0; k < maxZombiesGPerRoad; k++)
                            {
                                if(!RandChance(zombieGSpawnChance))
                                    continue;

                                var halfCellScale = cellScale / 2f;
                                
                                var spawnPos = 
                                    new Vector3(randomizer.Next((int)-halfCellScale, (int)halfCellScale),0,
                                        randomizer.Next((int)-halfCellScale, (int)halfCellScale));

                                var spawnRotation = Quaternion.Euler(new Vector3(0,randomizer.Next(0,360),0));

                                parentT.position = cellCenter + spawnPos;
                                parentT.rotation = spawnRotation;
                                
                                var zombie = Instantiate(roads.zombieG, parentT);
                                zombie.transform.parent = transform;
                            }
                        }
                    }
                    else
                    {
                        if(cellData.Item1 == 0)
                            continue;
                        
                        var miniCellScale = cellScale / 4f;
                            
                        void SpawnMiniCell(int x,int y)
                        {
                            parentT.position = new Vector3
                            ((j * cellScale - levelScale / 2 * cellScale) + miniCellScale * x, 0,
                                i * cellScale - levelScale / 2 * cellScale + miniCellScale * y);

                            parentT.rotation =
                                Quaternion.Euler(new Vector3(0, rotationMultiplier * randomizer.Next(0, 4), 0));
                            
                            var spawnedCityPart = Instantiate(roads.GetRoad
                                (cellData.Item1 - 1),parentT);

                            spawnedCityPart.transform.parent = transform;
                        }
                        
                        SpawnMiniCell(1,1);
                        SpawnMiniCell(1,-1);
                        SpawnMiniCell(-1,-1);
                        SpawnMiniCell(-1,1);
                    }
                }
            }
            
            DestroyImmediate(parentT.gameObject);
        }
    }
    
    [Serializable]
    public class RoadsPrefs
    {
        public GameObject[] forwardRoads;
        public GameObject[] threeTurnsRoads;
        public GameObject[] fourTurnsRoads;
        public GameObject[] turnRoads;

        [Space] public GameObject[] pavements;
        public GameObject[] pavementsBuildings;

        [Space] public GameObject[] borders;

        public GameObject zombie;
        public GameObject zombieG;
        
        public Random randomizer;

        public GameObject GetRoad(int id, bool isGreen = false)
        {
            switch (id)
            {
                case 0:
                    return !isGreen ? forwardRoads[0] : forwardRoads[1];
                
                case 1:
                    return !isGreen ? threeTurnsRoads[0] : threeTurnsRoads[1];

                case 2:
                {
                    var rand = randomizer.Next(0, 2);
                    
                    return !isGreen ? fourTurnsRoads[rand] : fourTurnsRoads[rand+2];
                }
                
                case 3:
                    return !isGreen ? turnRoads[0] : turnRoads[1];
                
                case 4:
                    return GetMiniCell();
                
                case 5:
                    return GetBorders();
            }

            throw new ArgumentException($"Wrong id! {id}");
        }

        public GameObject GetMiniCell()
        {
            var rand = randomizer.Next(0, 2);

            return rand switch
            {
                0 => GetPavement(),
                1 => GetPavementBuildings(),
                _ => null
            };
        }
        
        public GameObject GetPavement()
        {
            return pavements[randomizer.Next(0, pavements.Length)];
        }
        
        public GameObject GetPavementBuildings()
        {
            return pavementsBuildings[randomizer.Next(0, pavementsBuildings.Length)];
        }

        public GameObject GetBorders()
        {
            return borders[randomizer.Next(0, borders.Length)];
        }

    }
    
}

using UnityEditor;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    // Fields of all objects that need to be instantiated 
    [SerializeField]
    private GameObject pathObject;

    [SerializeField]
    private GameObject[] obstacleObjects;

    [SerializeField]
    private GameObject slimeObject;

    [SerializeField]
    private GameObject archObject;

    [SerializeField]
    private GameObject endObject;

    [SerializeField]
    private GameObject pickUpObject;

    [SerializeField]
    private GameObject coinObject;

    [SerializeField]
    private int maxChunks;

    private LevelData[] levelData;

    private Chunk[] chunks;

    // Current spawn values used to spawn chunks in the level
    private float currentSpawnX;
    private float currentSpawnZ;

    private GameObject obstacleWall;
    private GameObject slime;
    private GameObject path;
    private GameObject arch;

    public GameObject Player;

    private Direction currentDirection;
    private int Level = 0;

    private void Start()
    {
        Constants.MAX_CHUNKS = maxChunks;

        chunks = new Chunk[maxChunks];

        levelData = new LevelData[3];
        currentDirection = Direction.Straight;

        currentSpawnX = 0;
        currentSpawnZ = 0;

        GenerateLevel("Level " + Constants.LEVEL.ToString());
    }        

    // Procedurally generates levels and stores them as an asset
    public void GenerateLevel(string levelName)
    {
        levelData[Level] = ScriptableObject.CreateInstance<LevelData>();
        chunks[0] = Chunk.GetBasicChunk(new Chunk());

        // First chunk is always plain
        path = Instantiate(pathObject, transform, false);
        path.transform.position = new Vector3(currentSpawnX, 0, currentSpawnZ);
        currentSpawnZ += 40;

        for (int i = 1; i < maxChunks; i++)
        {
            chunks[i] = GenerateChunk();
            path = Instantiate(pathObject, transform, false);
            path.transform.position = new Vector3(currentSpawnX, 0, currentSpawnZ);
            path.transform.rotation = Quaternion.Euler(0, currentDirection == Direction.Straight ? 0 : 90, 0);

            // Spawning the required elements based on the chunk type and current spawn positions
            switch (chunks[i].Type)
            {
                case ChunkType.Obstacle:
                    {
                        ObstacleWall obstacle = chunks[i].Obstacle;
                        obstacleWall = Instantiate(obstacleObjects[obstacle.Type], transform, false);

                        if (currentDirection == Direction.Straight)
                        {
                            obstacleWall.transform.position = new Vector3(currentSpawnX - 2, 1.5f, currentSpawnZ - 20 + obstacle.SpawnPosition);
                        }
                        else if (currentDirection == Direction.Left)
                        {
                            obstacleWall.transform.rotation = Quaternion.Euler(0, -90, 0);
                            obstacleWall.transform.position = new Vector3(currentSpawnX + 20 - obstacle.SpawnPosition, 1.5f, currentSpawnZ - 2);
                        }
                        else
                        {
                            obstacleWall.transform.rotation = Quaternion.Euler(0, 90, 0);
                            obstacleWall.transform.position = new Vector3(currentSpawnX - 20 + obstacle.SpawnPosition, 1.5f, currentSpawnZ + 2);
                        }       
                        break;
                    }

                case ChunkType.Slime:
                    {
                        foreach (SlimePool slimePool in chunks[i].SlimePools)
                        {
                            slime = Instantiate(slimeObject, transform, false);

                            if (currentDirection == Direction.Straight)
                            {
                                slime.transform.position = new Vector3(slimePool.RelativeSpawnPositionX + currentSpawnX, 1.01f, currentSpawnZ - 5 + slimePool.RelativeSpawnPositionZ);
                            }
                            else if (currentDirection == Direction.Left)
                            {
                                slime.transform.rotation = Quaternion.Euler(0, 90, 0);
                                slime.transform.position = new Vector3(currentSpawnX + 5 - slimePool.RelativeSpawnPositionZ, 1.01f, slimePool.RelativeSpawnPositionX + currentSpawnZ);
                            }
                            else
                            {
                                slime.transform.rotation = Quaternion.Euler(0, 90, 0);
                                slime.transform.position = new Vector3(currentSpawnX - 5 + slimePool.RelativeSpawnPositionZ, 1.01f, slimePool.RelativeSpawnPositionX + currentSpawnZ);
                            }  
                        }

                        break;
                    }
                default:
                    {
                        // Spawn collectibles

                        int random = Random.Range(1, 6);
                        int step = 30 / random;
                        GameObject obj;
                        for (int j = 0; j < random; j++)
                        {
                            // More possibility to spawn coins
                            if (j % 4 == 0)
                            {
                                obj = pickUpObject;
                            }
                            else
                            {
                                obj = coinObject;
                            }

                            GameObject pickup = Instantiate(obj, transform, false);
                            if (currentDirection == Direction.Straight)
                            {
                                pickup.transform.position = new Vector3(Random.Range(-2, 3) + currentSpawnX, 1.5f , currentSpawnZ - 15 + step * j);
                            }
                            else if (currentDirection == Direction.Left)
                            {
                                pickup.transform.position = new Vector3(currentSpawnX + 15 - step * j, 1.5f, Random.Range(-2, 3) + currentSpawnZ);
                            }
                            else
                            {
                                pickup.transform.position = new Vector3(currentSpawnX - 15 + step * j, 1.5f , Random.Range(-2, 3) + currentSpawnZ);
                            }
                        }
                        break;
                    }
            }

            // No need to make a curve at the end of the last chunk
            if (i == maxChunks - 1)
            {
                break;
            }

            // Positioning of curves at every turn based on the current spawn values
            switch (chunks[i].TurnDirection)
            {
                case Direction.Left:
                    {
                        arch = Instantiate(archObject, transform, false);
                        if (currentDirection == Direction.Straight)
                        {
                            arch.transform.position = new Vector3(currentSpawnX - 2.5f, 0, currentSpawnZ + 25);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                            currentSpawnX -= 27.5f;
                            currentDirection = Direction.Left;
                        }
                        else if (currentDirection == Direction.Right)
                        {
                            arch.transform.position = new Vector3(currentSpawnX + 25, 0, currentSpawnZ + 2.5f);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 90, 0));
                            currentSpawnX += 27.5f;
                            currentDirection = Direction.Straight;
                        }

                        currentSpawnZ += 27.5f;
                        break;
                    }
                case Direction.Right:
                    {
                        arch = Instantiate(archObject, transform, false);
                        if (currentDirection == Direction.Straight)
                        {
                            arch.transform.position = new Vector3(currentSpawnX + 2.5f, 0, currentSpawnZ + 25);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, -90, 0)); 
                            currentSpawnX += 27.5f;
                            currentDirection = Direction.Right;
                        }
                        else if (currentDirection == Direction.Left)
                        {
                            arch.transform.position = new Vector3(currentSpawnX - 25, 0, currentSpawnZ + 2.5f);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 180, 0));
                            currentSpawnX -= 27.5f;
                            currentDirection = Direction.Straight;
                        }

                        currentSpawnZ += 27.5f;
                        break;
                    }
                default:
                    {
                        if (currentDirection == Direction.Straight )
                        {
                            currentSpawnZ += 40;
                        }
                        else if (currentDirection == Direction.Left)
                        {
                            currentSpawnX -= 40;
                        }
                        else
                        {
                            currentSpawnX += 40;
                        }

                        break;
                    }
            }
        }

        // Instantiating the end of the level based on current spawn values
        switch(currentDirection)
        {
            case Direction.Straight:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.position = new Vector3(currentSpawnX, 1.5f, currentSpawnZ + 25.5f);
                    end.transform.SetParent(transform);
                    break;
                }
            case Direction.Left:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.position = new Vector3(currentSpawnX - 25.5f, 1.5f, currentSpawnZ);
                    end.transform.rotation = Quaternion.Euler(0, -90, 0); 
                    end.transform.SetParent(transform);
                    break;
                }
            default:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.SetParent(transform);
                    end.transform.rotation = Quaternion.Euler(0, 90, 0);
                    end.transform.position = new Vector3(currentSpawnX + 25.5f, 1.5f, currentSpawnZ);
                    break;
                }
        }

        SaveLevel(levelName);     
    }

    // Saving level as an asset file on the editor
    private void SaveLevel(string levelName)
    {
        levelData[Level].Chunks = chunks;

#if UNITY_EDITOR
        EditorUtility.SetDirty(levelData[Level]);
        AssetDatabase.CreateAsset(levelData[Level], "Assets/Resources/Levels/" + levelName + ".asset");
        AssetDatabase.SaveAssets();
#endif
    }

    // Randomizes chunk everytime
    private Chunk GenerateChunk()
    {
        Chunk chunk = new Chunk();
        int step;

        chunk.Type = (ChunkType)Random.Range(0, 3);

        // Randomizes spawn positions of obstacles/pools to avoid cluttering and impossible levels
        switch(chunk.Type)
        {
            case ChunkType.Obstacle:
                {
                    chunk = Chunk.GetObstacleChunk(chunk);
                    chunk.Obstacle = new ObstacleWall();

                    chunk.Obstacle.Type = Random.Range(0, obstacleObjects.Length);
                    chunk.Obstacle.SpawnPosition = Random.Range(17, 24);           

                    chunk.SlimePools = null;
                    break;
                }
            case ChunkType.Slime:
                {
                    chunk = Chunk.GetSlimeChunk(chunk);
                    chunk.SlimePools = new SlimePool[Random.Range(1, 3)];
                    step = 30 / chunk.SlimePools.Length;

                    for (int i = 0; i < chunk.SlimePools.Length; i++)
                    {
                        chunk.SlimePools[i] = new SlimePool();
                        chunk.SlimePools[i].RelativeSpawnPositionX = Random.Range(-1, 2);
                        chunk.SlimePools[i].RelativeSpawnPositionZ = step * i;
                    }

                    chunk.Obstacle = null;
                    break;
                }
            default:
                {
                    chunk = Chunk.GetBasicChunk(chunk);
                    chunk.Obstacle = null;
                    chunk.SlimePools = null;
                    break;
                }
        }

        chunk.ShouldTurn = Random.Range(0, 2) == 0 ? true : false;

        if (chunk.ShouldTurn)
        {
            chunk.TurnDirection = Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right;

            // Avoid making two same turns back to back to avoid paths colliding with each other
            if (currentDirection == Direction.Left)
            {
                chunk.TurnDirection = Direction.Right;         
            }

            else if (currentDirection == Direction.Right)
            {
                chunk.TurnDirection = Direction.Left;
            }
        }
        else
        {
            chunk.TurnDirection = Direction.Straight;
        }

        return chunk;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject pathObject;

    [SerializeField]
    private GameObject obstacleObject;

    [SerializeField]
    private GameObject archObject;

    [SerializeField]
    private GameObject endObject;

    private LevelData[] levelData;

    private Chunk[] chunks;


    private float currentSpawnX;

    private float currentSpawnZ;

    private GameObject obstacle;
    private GameObject path;
    private GameObject arch;

    public Direction currentDirection;
    public int Level = 0;

    private void Start()
    {
        chunks = new Chunk[10];

        levelData = new LevelData[3];
        currentDirection = Direction.Straight;

        currentSpawnX = 0;
        currentSpawnZ = 0;

        GenerateLevel("Level " + Level.ToString());
    }        

    public void GenerateLevel(string levelName)
    {
        levelData[Level] = ScriptableObject.CreateInstance<LevelData>();
        chunks[0] = Chunk.GetBasicChunk(new Chunk());


        path = Instantiate(pathObject, transform, false);
        path.transform.position = new Vector3(currentSpawnX, 0, currentSpawnZ);
        currentSpawnZ += 30;

        for (int i = 1; i < 10; i++)
        {
            chunks[i] = GenerateChunk();
            path = Instantiate(pathObject, transform, false);
            path.transform.position = new Vector3(currentSpawnX, 0, currentSpawnZ);
            path.transform.rotation = Quaternion.Euler(0, currentDirection == Direction.Straight ? 0 : 90, 0);

            switch (chunks[i].Type)
            {
                case ChunkType.Obstacle:
                    {
                        break;
                    }

                case ChunkType.Slime:
                    {                        
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            if (i == 9)
            {
                break;
            }

            switch (chunks[i].TurnDirection)
            {
                case Direction.Left:
                    {
                        arch = Instantiate(archObject, transform, false);
                        if (currentDirection == Direction.Straight)
                        {
                            arch.transform.position = new Vector3(currentSpawnX - 2.5f, 0, currentSpawnZ + 20);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                            currentSpawnX -= 22.5f;
                            currentDirection = Direction.Left;
                        }
                        else if (currentDirection == Direction.Right)
                        {
                            arch.transform.position = new Vector3(currentSpawnX + 20, 0, currentSpawnZ + 2.5f);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 90, 0));
                            currentSpawnX += 22.5f;
                            currentDirection = Direction.Straight;
                        }

                        currentSpawnZ += 22.5f;
                        break;
                    }
                case Direction.Right:
                    {
                        arch = Instantiate(archObject, transform, false);
                        if (currentDirection == Direction.Straight)
                        {
                            arch.transform.position = new Vector3(currentSpawnX + 2.5f, 0, currentSpawnZ + 20);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, -90, 0)); 
                            currentSpawnX += 22.5f;
                            currentDirection = Direction.Right;
                        }
                        else if (currentDirection == Direction.Left)
                        {
                            arch.transform.position = new Vector3(currentSpawnX - 20, 0, currentSpawnZ + 2.5f);
                            arch.transform.rotation = Quaternion.Euler(new Vector3(90, 180, 0));
                            currentSpawnX -= 22.5f;
                            currentDirection = Direction.Straight;
                        }

                        currentSpawnZ += 22.5f;
                        break;
                    }
                default:
                    {
                        if (currentDirection == Direction.Straight )
                        {
                            currentSpawnZ += 30;
                        }
                        else if (currentDirection == Direction.Left)
                        {
                            currentSpawnX -= 30;
                        }
                        else
                        {
                            currentSpawnX += 30;
                        }

                        break;
                    }
            }
        }

        switch(currentDirection)
        {
            case Direction.Straight:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.position = new Vector3(currentSpawnX, 1.5f, currentSpawnZ + 20.5f);
                    end.transform.SetParent(transform);
                    break;
                }
            case Direction.Left:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.position = new Vector3(currentSpawnX - 20.5f, 1.5f, currentSpawnZ);
                    end.transform.rotation = Quaternion.Euler(0, -90, 0); 
                    end.transform.SetParent(transform);
                    break;
                }
            default:
                {
                    GameObject end = Instantiate(endObject, transform, false);
                    end.transform.SetParent(transform);
                    end.transform.rotation = Quaternion.Euler(0, 90, 0);
                    end.transform.position = new Vector3(currentSpawnX + 20.5f, 1.5f, currentSpawnZ);
                    break;
                }
        }

        SaveLevel(levelName);     
    }

    private void SaveLevel(string levelName)
    {
        levelData[Level].Chunks = chunks;
        EditorUtility.SetDirty(levelData[Level]);
        AssetDatabase.CreateAsset(levelData[Level], "Assets/Resources/Levels/" + levelName + ".asset");
        AssetDatabase.SaveAssets();
    }

    private Chunk GenerateChunk()
    {
        Chunk chunk = new Chunk();
        int random;

        random = Random.Range(1, 3);
        chunk.Type = (ChunkType)random;

        switch(chunk.Type)
        {
            case ChunkType.Obstacle:
                {
                    chunk = Chunk.GetObstacleChunk(chunk);
                    chunk.ObstacleCount = Random.Range(1, 6);
                    chunk.SlimeCount = 0;
                    break;
                }
            case ChunkType.Slime:
                {
                    chunk = Chunk.GetSlimeChunk(chunk);
                    chunk.ObstacleCount = 0;
                    chunk.SlimeCount = Random.Range(1, 4);
                    break;
                }
        }

        chunk.ShouldTurn = Random.Range(0, 2) == 0 ? true : false;

        if (chunk.ShouldTurn)
        {
            chunk.TurnDirection = Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right;

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

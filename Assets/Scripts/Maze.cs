using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EDirection
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3
}
public class Maze : MonoBehaviour
{
    public float prefabWidth = 8;
    public float percentage = 0.0f;
    public string loadingText;
    [Header("NavMesh")]
    private Cell[,] cells;
    public NavMeshSurface navMeshSurface;
    
    [Header("Prefabs")]
    public GameObject[] furniturePrefab;
    public GameObject[] decoPrefab;
    public GameObject playerPrefab;
    public GameObject startRoomPrefab;
    public GameObject endRoomPrefab;
    public GameObject floorPrefab;
    public GameObject rightFloorPrefab;
    public GameObject downFloorPrefab;
    public GameObject cornerFloorPrefab;
    public GameObject wallPrefab;

    [Header("Positions")]
    public Vector3 exitCellWorldPosRoom;
    public Vector2Int entranceCellPos;
    public Vector2Int exitCellPos;
    public Vector2Int mazeMiddleCellPos;
    private GameObject player;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
    }
    void Start()
    {
        player = Instantiate(playerPrefab, Vector3.up * 1000.0f, Quaternion.identity);
        player.GetComponent<Player>().enabled = false;
        StartCoroutine(DoTheMazeGeneration());
    }

    IEnumerator DoTheMazeGeneration()
    {
        yield return GenerateMazeFloor();
        yield return InitializeGrid();
        yield return GenerateMaze();
        yield return GenerateWalls(); 
        yield return NavMeshBuilder();
        yield return PlayerPos();
        LoadingScreen.Instance.CloseLoadingScreen();
        player.GetComponent<Player>().enabled = true;
    }

    public IEnumerator GenerateMazeFloor()
    {
        for (int i = 0; i < GameManagerInstance.Instance.size; i++)
        {
            for (int j = 0; j < GameManagerInstance.Instance.size; j++)
            {
                if (i == GameManagerInstance.Instance.size - 1 && j != GameManagerInstance.Instance.size - 1)
                {
                    Vector3 posRight = new Vector3(i * prefabWidth, 0, j * prefabWidth);
                    Instantiate(rightFloorPrefab, posRight, Quaternion.identity);
                }
                else if (j == GameManagerInstance.Instance.size - 1 && i != GameManagerInstance.Instance.size - 1)
                {
                    Vector3 posDown = new Vector3(i * prefabWidth, 0, j * prefabWidth);
                    Instantiate(downFloorPrefab, posDown, Quaternion.identity);
                }
                else if (i == GameManagerInstance.Instance.size - 1 && j == GameManagerInstance.Instance.size - 1)
                {
                    Vector3 posCorner = new Vector3(i * prefabWidth, 0, j * prefabWidth);
                    Instantiate(cornerFloorPrefab, posCorner, Quaternion.identity);
                }
                else
                {
                    Vector3 pos = new Vector3(i * prefabWidth, 0, j * prefabWidth);
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                }
            }

            if (i % (GameManagerInstance.Instance.size / 10) == 0)
            {
                percentage = percentage + 2.0f;
                percentage = Mathf.Clamp(percentage, 0, 20);
                loadingText = "Generating maze floor...";
                LoadingScreen.Instance.SetPercentage(percentage, loadingText);
                yield return null;
            }
        }

        percentage = 20;
        LoadingScreen.Instance.SetPercentage(percentage);
        yield return null;
    }

    public IEnumerator InitializeGrid()
    {
        cells = new Cell[GameManagerInstance.Instance.size, GameManagerInstance.Instance.size];
        for (int x = 0; x < GameManagerInstance.Instance.size; x++)
        {
            for (int y = 0; y < GameManagerInstance.Instance.size; y++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }
        percentage = 30.0f;
        loadingText = "Initializing grid...";
        LoadingScreen.Instance.SetPercentage(percentage, loadingText);
        yield return null;
    }
    public IEnumerator GenerateMaze()
    { 
        Stack<Cell> stack = new Stack<Cell>();
        Cell startCell = cells[0, 0];
        startCell.visited = true;
        
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            Cell currentCell = stack.Peek();
            List<Cell> unvisitedNeighbors = GetCells(currentCell);
            if (unvisitedNeighbors.Count > 0)
            {
                Cell nextCell = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                RemoveWalls(currentCell, nextCell);
                nextCell.visited = true;
                stack.Push(nextCell);
            }
            else
            {
                stack.Pop();
            }
        }
        Cell entranceCell = cells[0, Random.Range(0, GameManagerInstance.Instance.size)];
        entranceCellPos = entranceCell.position; 
        entranceCell.neighbors[(int)EDirection.LEFT] = false;
        
        Cell exitCell = cells[GameManagerInstance.Instance.size - 1, Random.Range(0, GameManagerInstance.Instance.size)];
        exitCellPos = exitCell.position;
        exitCell.neighbors[(int)EDirection.RIGHT] = false;
        
        Cell mazeMiddleCell = cells[GameManagerInstance.Instance.size / 2, GameManagerInstance.Instance.size / 2];
        mazeMiddleCellPos = mazeMiddleCell.position;
        
        Vector3 entranceCellWorldPosRoom = new Vector3(entranceCellPos.y * prefabWidth, 0, (entranceCellPos.x * prefabWidth) - prefabWidth);
        Instantiate(startRoomPrefab, entranceCellWorldPosRoom, Quaternion.identity);
        
        exitCellWorldPosRoom = new Vector3(exitCellPos.y * prefabWidth, 0, (exitCellPos.x * prefabWidth) + prefabWidth);
        Quaternion rotate = Quaternion.Euler(0f, 180f, 0f);
        Instantiate(endRoomPrefab, exitCellWorldPosRoom, rotate, transform);
        percentage = 45.0f;
        loadingText = "Generating maze...";
        LoadingScreen.Instance.SetPercentage(percentage, loadingText);
        yield return null;
    }


    public IEnumerator GenerateWalls()
    {
        for (int i = 0; i < GameManagerInstance.Instance.size; i++)
        {
            for (int j = 0; j < GameManagerInstance.Instance.size; j++)
            {
                Cell cell = cells[i, j];
                Vector3 cellPos = new Vector3(j * prefabWidth, 0, i * prefabWidth);
                
                if (cell.neighbors[(int)EDirection.UP])
                {
                    Vector3 wallPos = cellPos + (-Vector3.right * prefabWidth / 2f);
                    Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                    Instantiate(wallPrefab, wallPos, rotation, transform);
                    
                    int random = Random.Range(0, 8);
                    if (random == 0)
                    {
                        int randomFurniture = Random.Range(0, furniturePrefab.Length);
                        Vector3 cellWarPos = new Vector3((j * prefabWidth) + 1, 0, i * prefabWidth);
                        Vector3 furniturePos = cellWarPos + (-Vector3.right * prefabWidth / 2f);
                        Quaternion rot = Quaternion.Euler(0f, 90f, 0f);
                        Instantiate(furniturePrefab[randomFurniture], furniturePos, rot, transform);
                        
                        int randomDecoSpawn = Random.Range(0, 2);
                        if (randomDecoSpawn == 0)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x, furniturePos.y, furniturePos.z + 2);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                        else if (randomDecoSpawn == 1)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x, furniturePos.y, furniturePos.z - 2);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                    }
                }
                
                if (j == GameManagerInstance.Instance.size - 1 && cell.neighbors[(int)EDirection.DOWN]) 
                {
                    Vector3 wallPos = cellPos + (Vector3.right * prefabWidth / 2f);
                    Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
                    Instantiate(wallPrefab, wallPos, rotation, transform);
                    
                    int random = Random.Range(0, 8);
                    if (random == 0)
                    {
                        int randomFurniture = Random.Range(0, furniturePrefab.Length);
                        Vector3 cellWarPos = new Vector3((j * prefabWidth) - 1, 0, i * prefabWidth);
                        Vector3 furniturePos = cellWarPos + (Vector3.right * prefabWidth / 2f);
                        Quaternion rot = Quaternion.Euler(0f, -90f, 0f);
                        Instantiate(furniturePrefab[randomFurniture], furniturePos, rot, transform);
                        
                        int randomDecoSpawn = Random.Range(0, 2);
                        if (randomDecoSpawn == 0)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x, furniturePos.y, furniturePos.z - 2);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, -90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                        else if (randomDecoSpawn == 1)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x, furniturePos.y, furniturePos.z + 2);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, -90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                    }
                }
                
                if ( cell.neighbors[(int)EDirection.LEFT])
                {
                    Vector3 wallPos = cellPos + (-Vector3.forward * prefabWidth / 2f);
                    Quaternion rotation = Quaternion.Euler(0f, 90f, 0f);
                    Instantiate(wallPrefab, wallPos, rotation, transform);
                    
                    int random = Random.Range(0, 8);
                    if (random == 0)
                    {
                        int randomFurniture = Random.Range(0, furniturePrefab.Length);
                        Vector3 cellWarPos = new Vector3((j * prefabWidth), 0, (i * prefabWidth) + 1);
                        Vector3 furniturePos = cellWarPos + (-Vector3.forward * prefabWidth / 2f);
                        Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
                        Instantiate(furniturePrefab[randomFurniture], furniturePos, rot, transform);
                        
                        int randomDecoSpawn = Random.Range(0, 2);
                        if (randomDecoSpawn == 0)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x - 2, furniturePos.y, furniturePos.z);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                        else if (randomDecoSpawn == 1)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x + 2, furniturePos.y, furniturePos.z);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                    }
                }
                
                if (i == GameManagerInstance.Instance.size - 1 && cell.neighbors[(int)EDirection.RIGHT])
                {
                    Vector3 wallPos = cellPos + (Vector3.forward * prefabWidth / 2f);
                    Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);
                    Instantiate(wallPrefab, wallPos, rotation, transform);
                    
                    int random = Random.Range(0, 8);
                    if (random == 0)
                    {
                        int randomFurniture = Random.Range(0, furniturePrefab.Length);
                        Vector3 cellWarPos = new Vector3((j * prefabWidth), 0, (i * prefabWidth - 1));
                        Vector3 furniturePos = cellWarPos + (Vector3.forward * prefabWidth / 2f);
                        Quaternion rot = Quaternion.Euler(0f, 180f, 0f);
                        Instantiate(furniturePrefab[randomFurniture], furniturePos, rot, transform);
                        
                        int randomDecoSpawn = Random.Range(0, 2);
                        if (randomDecoSpawn == 0)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x + 2, furniturePos.y, furniturePos.z);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                        else if (randomDecoSpawn == 1)
                        {
                            int randomDeco = Random.Range(0, decoPrefab.Length);
                            Vector3 decoPos = new Vector3(furniturePos.x - 2, furniturePos.y, furniturePos.z);
                            Quaternion decoRot = Quaternion.Euler(0f, Random.Range(0.0f, 90.0f), 0f);
                            Instantiate(decoPrefab[randomDeco], decoPos, decoRot, transform);
                        }
                    }
                }
            }

            if (i % (GameManagerInstance.Instance.size / 5) == 0)
            {
                percentage = percentage + 1.25f;
                percentage = Mathf.Clamp(percentage, 45.0f, 70.0f);
                loadingText = "Generating walls...";
                LoadingScreen.Instance.SetPercentage(percentage, loadingText);
                yield return null;
            }
        }

        percentage = 70.0f;
        loadingText = "Generating NavMesh...";
        LoadingScreen.Instance.SetPercentage(percentage, loadingText);
        yield return null;
    }
    
    public IEnumerator NavMeshBuilder()
    {
        //UnityEngine.AI.NavMeshBuilder.UpdateNavMeshDataAsync()
        //navMeshSurface.BuildNavMesh();
        yield return NavMeshOutOfDateCoroutine(Vector3.zero, 2000.0f, true);
        percentage = 90.0f;
        loadingText = "Setting player position...";
        LoadingScreen.Instance.SetPercentage(percentage, loadingText);
        yield return null;
    }
    /// <summary>
/// Coroutine to rebuild the current Scene NavMesh.
/// </summary>
/// <param name="playerPosition">The center of the mesh search volume</param>
/// <param name="navigationMeshRadius">How big a volume should we search for surfaces in.</param>
/// <param name="rebuildAll">If "true", delete any existing meshes before adding new ones.</param>
/// <returns></returns>
IEnumerator NavMeshOutOfDateCoroutine(Vector3 playerPosition, float navigationMeshRadius, bool rebuildAll)
    {
        // Get the list of all "sources" around us.  This is basically little gridded subsquares
        // of our terrains.
        List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();

        // Set up a boundary area for the build sources collector to look at;
        Bounds patchBounds = new Bounds(playerPosition,
            new Vector3(navigationMeshRadius, navigationMeshRadius, navigationMeshRadius));

        // This actually collects the potential surfaces.
        UnityEngine.AI.NavMeshBuilder.CollectSources(
            patchBounds,
            1 << LayerMask.NameToLayer("Default"),
            NavMeshCollectGeometry.PhysicsColliders,
            0,
            new List<NavMeshBuildMarkup>(),
            buildSources);

        yield return null;

        // Build some empty NavMeshData objects
        int numAgentTypes = NavMesh.GetSettingsCount();
        NavMeshData[] meshData = new NavMeshData[numAgentTypes];

        for (int agentIndex = 0; agentIndex < numAgentTypes; agentIndex++)
        {
            // Get the settings for each of our agent "sizes" (humanoid, giant humanoid)
            NavMeshBuildSettings bSettings = NavMesh.GetSettingsByIndex(agentIndex);

            // If there are any issues with the agent, print them out as a warning.
#if DEBUG
            foreach (string s in bSettings.ValidationReport(patchBounds))
            {
                Debug.LogWarning($"BuildSettings Report: {NavMesh.GetSettingsNameFromID(bSettings.agentTypeID)} : {s}");
            }
#endif
            
            // Make empty mesh data object.
            meshData[agentIndex] = new NavMeshData();

            AsyncOperation buildOp = UnityEngine.AI.NavMeshBuilder.UpdateNavMeshDataAsync(meshData[agentIndex], bSettings, buildSources, patchBounds);

            while (!buildOp.isDone) yield return null;
        }

        if (rebuildAll)
        {
            NavMesh.RemoveAllNavMeshData();
        }

        for (int nmd = 0; nmd < meshData.Length; nmd++)
        {
            NavMesh.AddNavMeshData(meshData[nmd]);
        }

        yield return null;
    }

    public IEnumerator PlayerPos()
    {
        float gridWidth = 8f;
        Vector2Int startCell = entranceCellPos;
        Vector3 entranceCellWorldPos = new Vector3(startCell.y * gridWidth, 0.0f, (startCell.x * gridWidth) - gridWidth);
        player.transform.position = entranceCellWorldPos;
        percentage = 99.0f;
        loadingText = "Setting player position...";
        LoadingScreen.Instance.SetPercentage(percentage, loadingText);
        yield return null;
    }
   
    public void RemoveWalls(Cell currentCell, Cell nextCell)
    {
        int xDiff = nextCell.position.x - currentCell.position.x;
        int yDiff = nextCell.position.y - currentCell.position.y;

        if (xDiff == 1)
        {
            currentCell.neighbors[(int)EDirection.RIGHT] = false;
            nextCell.neighbors[(int)EDirection.LEFT] = false;
        }
        else if (xDiff == -1)
        {
            currentCell.neighbors[(int)EDirection.LEFT] = false;
            nextCell.neighbors[(int)EDirection.RIGHT] = false;
        }
        else if (yDiff == 1)
        {
            currentCell.neighbors[(int)EDirection.DOWN] = false;
            nextCell.neighbors[(int)EDirection.UP] = false;
        } 
        else if (yDiff == -1)
        {
            currentCell.neighbors[(int)EDirection.UP] = false;
            nextCell.neighbors[(int)EDirection.DOWN] = false;
        }
        else
        {
            Debug.LogError("OH NO");
        }
    }

    public List<Cell> GetCells(Cell cell)
    {
        List<Cell> neighborsVisited = new List<Cell>();
        int x = cell.position.x;
        int y = cell.position.y;

        if (x > 0 &&  !cells[x - 1, y].visited)
        {
            neighborsVisited.Add(cells[x - 1, y]);
        }

        if (x < GameManagerInstance.Instance.size - 1 && !cells[x + 1, y].visited)
        {
            neighborsVisited.Add(cells[x + 1, y]);
        }

        if (y > 0 && !cells[x, y - 1].visited)
        {
            neighborsVisited.Add(cells[x, y - 1]);
        }

        if (y < GameManagerInstance.Instance.size - 1 && !cells[x, y + 1].visited)
        {
            neighborsVisited.Add(cells[x, y + 1]);
        }
        return neighborsVisited;
    }
  
    public class Cell
    {
        public bool visited;
        public bool[] neighbors;
        public Vector2Int position;
        
        public Cell(int x, int y)
        {
            visited = false;
            neighbors = new bool[]{true, true, true, true};
            position = new Vector2Int(x, y);
        }
    }
}
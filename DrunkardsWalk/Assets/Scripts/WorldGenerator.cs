using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    WorldMesh meshGenerator;
    WorldCell[] cells;
    List<WorldCell> openCells;
    List<Room> rooms;

    private void Awake()
    {
        rooms = new List<Room>();
        openCells = new List<WorldCell>();
        cells = new WorldCell[Metrics.worldSize.x * Metrics.worldSize.y];
        meshGenerator = GetComponentInChildren<WorldMesh>();
        if (meshGenerator == null)
        {
            Debug.LogError("Component WorldMesh not found in + " + gameObject.name);
        }
    }

    private void Start()
    {
        GenerateWorldDrunkenWalk();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReGenerateWorldDrunkenWalk();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("World generated");
        }
    }

    void GenerateWorldDrunkenWalk()
    {
        GenerateWorld();

        // Open center cell
        Vector2Int coords = new Vector2Int(Metrics.worldSize.x / 2, Metrics.worldSize.y / 2);

        int drunkCounter = 0;
        WorldCell centerCell = GetCellFromCoordinates(coords);
        centerCell.Open();
        openCells.Add(centerCell);

        while(drunkCounter < Metrics.walks)
        {
            GenerateDrunkPath();
            drunkCounter++;
        }

        meshGenerator.GenerateMesh(cells);
    }

    void ReGenerateWorldDrunkenWalk()
    {
        CleanCells();
        openCells.Clear();
        GenerateWorldDrunkenWalk();
    }

    void GenerateWorldBSP()
    {
        GenerateWorld();

        RoomTreeNode partitionTree = ChopSpace();
        GenerateAllRoomsBSP(partitionTree);
        GenerateAllCorridors();

        meshGenerator.GenerateMesh(cells);
    }

    void RegenerateWorldBSP()
    {
        CleanCells();
        rooms.Clear();
        GenerateWorldBSP();
    }

    void RegenerateWorldCA()
    {
        CleanCells();
        rooms.Clear();
        GenerateWorldCA();
    }

    void GenerateWorldCA()
    {
        GenerateWorld();
        OrderWorld();
        meshGenerator.GenerateMesh(cells);
    }

    void OrderWorld()
    {
        foreach(WorldCell cell in cells)
        {
            int amount = cell.EmptyNeightbors();
            if(amount <= 0 || amount > 4)
            {
                cell.Close();
            }
            else
            {
                cell.Open();
            }
        }
    }

    void GenerateDrunkPath()
    {
        // Get a random open cell
        WorldCell curentCell = openCells[Random.Range(0, openCells.Count)];
        int walkLenght = 0;

        while(walkLenght <= Metrics.walkDistance)
        {
            // Select a random neigtbor
            curentCell = curentCell.GetRandomNeightbor();
            if (curentCell == null) // Got to a border
            {
                return;
            }
            curentCell.Open(); // Open new cell
            openCells.Add(curentCell);
            walkLenght++;
        }
    }

    void RegenerateWorldBasicRoomGeneration()
    {
        CleanCells();
        GenerateWorld();
        GenerateAllRooms();
        GenerateAllCorridors();

        meshGenerator.GenerateMesh(cells);
    }

    void GenerateWorld()
    {
        for(int y = 0; y < Metrics.worldSize.y; y++)
        {
            for(int x = 0; x < Metrics.worldSize.x; x++)
            {
                int index = x + y * Metrics.worldSize.x;
                GameObject newCell = Instantiate(cellPrefab,transform);
                WorldCell cell = newCell.GetComponent<WorldCell>();
                cell.Instantiate(x, y, Metrics.cellSize);
                cells[index] = cell;

                // Has something on left (not 1st column)
                if(x!= 0 || x % Metrics.worldSize.x != 0)
                {
                    cell.SetNeightbor(cells[index - 1], Neightbors.Left);
                }
                // Has somthing below (not 1st row)
                if(y!= 0)
                {
                    cell.SetNeightbor(cells[index - Metrics.worldSize.x], Neightbors.Down);
                    // Has something below left (not 1st column)
                    if (x != 0 || x % Metrics.worldSize.x != 0)
                    {
                        cell.SetNeightbor(cells[index - Metrics.worldSize.x - 1], Neightbors.DownLeft);
                    }
                    // Has something below right (not last column)
                    if ((x + 1) % Metrics.worldSize.x != 0)
                    {
                        cell.SetNeightbor(cells[index - Metrics.worldSize.x + 1], Neightbors.DownRight);
                    }
                }
            }
        }
    }

    RoomTreeNode ChopSpace()
    {
        RoomTreeNode worldPartition = new RoomTreeNode(cells, Metrics.worldSize);
        RoomTreeNode[] leafPartitions;
        int partitionCounter = 0;

        while (partitionCounter < Metrics.desiredPartitionAmount)
        {
            leafPartitions = worldPartition.GetLeafs();
            foreach (RoomTreeNode node in leafPartitions)
            {
                node.Chop();
            }
            partitionCounter++;
        }

        return worldPartition;
    }

    void GenerateAllRoomsBSP(RoomTreeNode tree)
    {
        RoomTreeNode[] leafPartitions = tree.GetLeafs();
        foreach (RoomTreeNode node in leafPartitions)
        {
            rooms.Add(node.GenerateBSPRoom());
        }
    }

    void CleanCells()
    {
        for(int aux = cells.Length - 1; aux >= 0; aux--)
        {
            Destroy(cells[aux].gameObject);
        }
    }

    void GenerateAllRooms()
    {
        rooms.Clear();
        int amountOfRooms = 0;
        while(amountOfRooms < Metrics.amountOfRooms)
        {
            if (GenerateRoom())
            {
                amountOfRooms++;
            }
        }
    }

    bool GenerateRoom()
    {
        Room newRoom = new Room();
        newRoom.GenerateRandomRoom();
        if (newRoom.CalculateRoomCells(cells))
        {
            rooms.Add(newRoom);
            return true;
        }
        else
        {
            return false;
        }
    }

    void GenerateAllCorridorsNonLineal() 
    {
        for(int exp = 1; exp <= Metrics.desiredPartitionAmount; exp++)
        {
            for (int index = 0; index < rooms.Count; )
            {
                int jump = (int)Mathf.Pow(2, exp);
                GenerateCorridor(rooms[index].GetCenter(), rooms[index + jump - 1].GetCenter());
                index = index + jump; 
            }
        }
    }

    void GenerateAllCorridors()
    {
        rooms.Sort();

        for(int roomCorridor = 0; roomCorridor < rooms.Count - 1; roomCorridor++)
        {
            GenerateCorridor(rooms[roomCorridor].GetCenter(), rooms[roomCorridor + 1].GetCenter());
        }
    }

    void GenerateCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int halfCorridorCoords;
        if (Random.Range(0, 2) == 0) // Horizontal 1st
        {
            halfCorridorCoords = HorizontalCorridor(start, end);
            VertialCorridor(halfCorridorCoords, end);
        }
        else // Vertical 1st
        {
            halfCorridorCoords = VertialCorridor(start, end);
            HorizontalCorridor(halfCorridorCoords, end);
        }
    }

    Vector2Int HorizontalCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int currentPos = start;
        while(currentPos.x != end.x)
        {
            if(currentPos.x < end.x)
            {
                currentPos.x++;
            }
            else
            {
                currentPos.x--;
            }

            WorldCell currentCell = GetCellFromCoordinates(currentPos);
            currentCell.Fill();
            currentCell.SetColor(Color.white);
        }
        return currentPos;
    }


    Vector2Int VertialCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int currentPos = start;
        while (currentPos.y != end.y)
        {
            if (currentPos.y < end.y)
            {
                currentPos.y++;
            }
            else
            {
                currentPos.y--;
            }

            WorldCell currentCell = GetCellFromCoordinates(currentPos);
            currentCell.Fill();
            currentCell.SetColor(Color.white);
        }
        return currentPos;
    }

    WorldCell GetCellFromCoordinates(Vector2Int coords)
    {
        return cells[coords.x + coords.y * Metrics.worldSize.x];
    }
}

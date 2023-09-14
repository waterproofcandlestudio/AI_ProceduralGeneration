using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : IComparable<Room>
{
    Vector2Int roomCenter;
    Vector2Int roomSize;
    Color roomColor;
    WorldCell[] roomCells;

    public Vector2Int GetCenter()
    {
        return roomCenter;
    }

    public void GenerateRandomRoom()
    {
        roomSize = new Vector2Int(UnityEngine.Random.Range(Metrics.minRoomsSize.x, Metrics.maxRoomsSize.x + 1),
                                  UnityEngine.Random.Range(Metrics.minRoomsSize.y, Metrics.maxRoomsSize.y + 1));

        roomColor = Color.white;

        roomCenter = new Vector2Int(UnityEngine.Random.Range(0, Metrics.worldSize.x) ,
                                    UnityEngine.Random.Range(0, Metrics.worldSize.y));
    }

    public void GenerateBSPRoom(int id, Vector2Int newRoomCenter, Vector2Int region, WorldCell[] partitionCells)
    {
        roomCenter = newRoomCenter;
        int shortValue = region.x < region.y ? region.x : region.y;
        shortValue -= 2;
        roomSize = new Vector2Int(shortValue, shortValue);
        roomColor = Color.white;
        CalculateRoomCellsBSP(partitionCells, region);

    }

    public void DebugGenerateRoomOnCenter(WorldCell centerCell)
    {
        centerCell.Fill();
        centerCell.SetColor(Color.white);
        Debug.LogError("This method is for debug purposes and shouldnt be called");
    }

    private void CalculateRoomCellsBSP(WorldCell[] cells, Vector2Int regionSize)
    {
        List<WorldCell> roomCellsList = new List<WorldCell>();

        int index;
        Vector2Int leftCornerCoords = new Vector2Int(roomCenter.x - roomSize.x/2, roomCenter.y - roomSize.y/2);
        for(index = 0; index < cells.Length; index++)
        {
            if (cells[index].GetCoordinates() == leftCornerCoords)
            {
                break;
            }
        }

        for(int yIndex = 0; yIndex < roomSize.y; yIndex++)
        {
            for (int xIndex = 0; xIndex < roomSize.x; xIndex++)
            {
                roomCellsList.Add(cells[(index + xIndex) + yIndex * regionSize.x]);
            }
        }
        
        foreach(WorldCell cell in roomCellsList)
        {
            cell.SetColor(roomColor);
            cell.Fill();
        }
    }

    public bool CalculateRoomCells(WorldCell[] cells)
    {
        List<WorldCell> cellList = new List<WorldCell>();
        for(int roomCoordsY = 0; roomCoordsY < roomSize.y; roomCoordsY++)
        {
            for (int roomCoordsX = 0; roomCoordsX < roomSize.x; roomCoordsX++)
            {
                int cellIndexX = roomCenter.x - (int)(roomSize.x * 0.5f);
                int cellIndexY = roomCenter.y - (int)(roomSize.y * 0.5f);

                if((cellIndexX < Metrics.worldSize.x && cellIndexX >= 0) && 
                   (cellIndexY < Metrics.worldSize.y && cellIndexY >= 0))
                {
                    WorldCell newCell = cells[Mathf.Clamp((roomCoordsX + cellIndexX), 0 , Metrics.worldSize.x - 1) + 
                                              Mathf.Clamp(roomCoordsY + cellIndexY, 0 , Metrics.worldSize.y-1) * 
                                              Metrics.worldSize.x];
                    if (newCell.IsEmpty())
                    {
                        if (!cellList.Contains(newCell))
                        {
                            cellList.Add(newCell);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        if(cellList.Count == 0)
        {
            return false;
        }
        roomCells = cellList.ToArray();
        foreach(WorldCell cell in roomCells) 
        {
            cell.Fill();
            cell.SetColor(roomColor);
        }

        return true;
    }

    public int CompareTo(Room other)
    {
        return this.GetCenter().x.CompareTo(other.GetCenter().x);
    }
}

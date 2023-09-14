using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WorldCell : MonoBehaviour
{
    WorldCell[] neightbors;
    Vector2Int coordinates;
    Vector2 size;
    Color cellColor;
    bool isEmpty;

    public void Instantiate(int x, int y, Vector2 cellSize)
    {
        size = cellSize;
        coordinates = new Vector2Int(x,y);
        transform.position = new Vector3(size.x * coordinates.x, 0, size.y * coordinates.y);

        /* Celllar random open or close
        if (Random.Range(0, 2) == 0)
        {
            Open();
        }
        else 
        {
            Close();
        }
        */

        Close();
        
        neightbors = new WorldCell[Metrics.neightborAmount];
    }

    public Color GetColor()
    {
        return cellColor;
    }
    public void SetColor(Color newColor)
    {
        cellColor = newColor;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public void Fill()
    {
        isEmpty = false;
    }

    public void Close()
    {
        isEmpty = false;
        SetColor(Color.black);
    }

    public void Open()
    {
        isEmpty = true;
        SetColor(Color.white);
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }


    public void SetNeightbor(WorldCell cell, Neightbors neightborPosition)
    {
        if(cell == null)
        {
            Debug.LogError("Cell as argument to set neightbor is null");
            return;
        }
        neightbors[(int)neightborPosition] = cell; // Vecino ida
        cell.neightbors[(int)neightborPosition + (int)(Metrics.neightborAmount * 0.5f)] = this; // Vecino vuelta
    }

    public int EmptyNeightbors()
    {
        if (IsBorder())
        {
            return -1;
        }

        int counter = 0;
        foreach(WorldCell cell in neightbors)
        {
            if(cell == null || cell.IsEmpty())
            {
                counter++;
            }
        }
        
        return counter;
    }

    public bool IsBorder()
    {
        int counter = 0;
        foreach (WorldCell cell in neightbors)
        {
            if (cell == null)
            {
                counter++;
            }
        }
        if(counter >= 3)
        {
            return true;
        }
        return false;
    }

    public WorldCell GetRandomNeightbor()
    {
        return neightbors[Random.Range(0, neightbors.Length)];
    }
}

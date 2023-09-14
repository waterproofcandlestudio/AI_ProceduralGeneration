using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Metrics
{
    // Global variables
    public static Vector2 cellSize = new Vector2(10f,10f);
    public static Vector2Int worldSize  = new Vector2Int(50, 50);
    public static Vector2Int minRoomsSize = new Vector2Int(5, 5);
    public static Vector2Int maxRoomsSize = new Vector2Int(15, 15);
    public static int amountOfRooms = 5;

    // BSP variables
    public static int desiredPartitionAmount = 3;

    // Celullar automata
    public static int neightborAmount = 8;

    // Drunken walk
    public static int walks = 20;
    public static int walkDistance = 50;

}

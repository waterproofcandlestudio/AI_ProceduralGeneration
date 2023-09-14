using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTreeNode
{
    RoomTreeNode left;
    RoomTreeNode right;

    bool horizontal;
    WorldCell[] cells;
    Vector2Int partitionSize;
    Vector2Int partitionCenter;

    public RoomTreeNode(WorldCell[] newCells, Vector2Int newPartitionSize)
    {
        cells = newCells;
        partitionCenter = new Vector2Int(cells[0].GetCoordinates().x + (int)(newPartitionSize.x / 2), 
                                         cells[0].GetCoordinates().y + (int)(newPartitionSize.y / 2));

        partitionSize = newPartitionSize;
        horizontal = Random.Range(0, 2) == 0 ? true : false;
        left = null;
        right = null;
    }

    public void Chop()
    {
        if (horizontal)
        {
            ChopHorizontal();
        }
        else
        {
            ChopVertical();
        }
    }

    private void ChopHorizontal()
    {
        // Consider if the partition is big enought
        int random = Random.Range(2, 5);
        int yChopRow = (int)(partitionSize.y / random);
        yChopRow = Random.Range(0, 2) == 0 ? yChopRow : partitionSize.y - yChopRow;
        List<WorldCell> cellsUp   = new List<WorldCell>();
        List<WorldCell> cellsDown = new List<WorldCell>();
        Vector2Int sizeDown = new Vector2Int(partitionSize.x, yChopRow);
        Vector2Int sizeUp   = new Vector2Int(partitionSize.x, partitionSize.y - sizeDown.y);

        for (int index = 0; index < cells.Length; index++)
        {
            if ((int)(index / partitionSize.x) >= yChopRow)
            {
                cellsUp.Add(cells[index]);
            }
            else
            {
                cellsDown.Add(cells[index]);
            }
        }

        left = new RoomTreeNode(cellsUp.ToArray(), sizeUp);
        right = new RoomTreeNode(cellsDown.ToArray(), sizeDown);
    }

    private void ChopVertical()
    {
        // Consider if the partition is big enought
        int random = Random.Range(2, 5);
        int xChopColumn = (int)(partitionSize.x / random);
        xChopColumn = Random.Range(0, 2) == 0 ? xChopColumn : partitionSize.x - xChopColumn;
        List<WorldCell> cellsLeft  = new List<WorldCell>();
        List<WorldCell> cellsRight = new List<WorldCell>();
        Vector2Int sizeRight = new Vector2Int(partitionSize.x - xChopColumn, partitionSize.y);
        Vector2Int sizeLeft  = new Vector2Int(partitionSize.x - sizeRight.x, partitionSize.y);

        for(int index = 0; index < cells.Length; index++)
        {
            if(index%partitionSize.x >= xChopColumn)
            {
                cellsRight.Add(cells[index]);
            }
            else
            {
                cellsLeft.Add(cells[index]);
            }
        }

        left  = new RoomTreeNode(cellsLeft.ToArray(), sizeLeft);
        right = new RoomTreeNode(cellsRight.ToArray(), sizeRight);
    }

    public Room GenerateBSPRoom(int id = 0)
    {
        Room room = new Room();
        room.GenerateBSPRoom(id, partitionCenter, partitionSize, cells);

        return room;
    }

    private bool IsLeaf()
    {
        if(left == null && right == null)
        {
            return true;
        }
        return false;
    }

    public RoomTreeNode[] GetLeafs()
    {
        List<RoomTreeNode> leafList = new List<RoomTreeNode>();
        CheckTreeForLeafs(this, leafList);
        return leafList.ToArray();
    }

    private void CheckTreeForLeafs(RoomTreeNode node, List<RoomTreeNode> leafs)
    {
        if (node.IsLeaf())
        {
            leafs.Add(node);
            return;
        }
        if(left != null)
        {
            CheckTreeForLeafs(node.left, leafs);
        }
        if(right != null)
        {
            CheckTreeForLeafs(node.right, leafs);
        }
    }

}

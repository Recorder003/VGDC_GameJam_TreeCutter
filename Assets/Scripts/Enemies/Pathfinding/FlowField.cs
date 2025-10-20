using System;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Cell[,] cells;
    public Vector2Int gridSize;
    public float cellRadius;

    public float cellDiameter;
    public Cell destinationCell;

    public FlowField(float _cellSize, Vector2Int _gridSize)
    {
        this.cellRadius = _cellSize;
        this.gridSize = _gridSize;
        cellDiameter = cellRadius * 2f;
    }

    public void CreateGrid()
    {
        cells = new Cell[gridSize.x, gridSize.y];


        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPosition = new Vector3(x * cellDiameter + cellRadius, y * cellDiameter + cellRadius, 0);
                cells[x, y] = new Cell(worldPosition, new Vector2Int(x, y));
            }
        }
    }

    public void CreateCostField()
    {
        Vector3 cellHalf = Vector3.one * cellRadius;
        int terrainLayer = LayerMask.GetMask("Terrain", "Obstacle");
        foreach (Cell cell in cells)
        {
            Collider2D hit = Physics2D.OverlapBox(cell.worldPos, cellHalf, 0, terrainLayer);
            if (hit != null)
            {
                Console.Write("Hit " + hit.gameObject.name + " on layer " + LayerMask.LayerToName(hit.gameObject.layer));
                switch (hit.gameObject.layer)
                {
                    case 8: // Obstacle
                        cell.increaseCost(int.MaxValue);
                        break;
                    case 6: // Terrain
                        cell.increaseCost(0);
                        break;
                }
            }

        }

    }

    public void CreateIntegrationField(Cell _destinationCell)
    {
        destinationCell = _destinationCell;

        destinationCell.cost = 0;
        destinationCell.bestCost = 0;

        Queue<Cell> cellsToCheck = new Queue<Cell>();
        cellsToCheck.Enqueue(destinationCell);

        while (cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> cellNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);

            foreach (Cell neighbor in cellNeighbors)
            {
                if (neighbor.cost == int.MaxValue)
                    continue;
                if (neighbor.cost + curCell.bestCost < neighbor.bestCost)
                {
                    neighbor.bestCost = (ushort)(neighbor.cost + curCell.bestCost);
                    cellsToCheck.Enqueue(neighbor);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach (Cell cell in cells)
        {
            List<Cell> cellNeighbors = GetNeighborCells(cell.gridIndex, GridDirection.AllDirections);

            int bestCost = cell.bestCost;

            foreach (Cell neighbor in cellNeighbors)
            {
                if (neighbor.bestCost < bestCost)
                {
                    bestCost = neighbor.bestCost;
                    cell.bestDirection = GridDirection.GetDirectionFromV2I(neighbor.gridIndex - cell.gridIndex);
                }
            }

        }
    }

    private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
    {
        List<Cell> neighbors = new List<Cell>();

        foreach (Vector2Int curDirection in directions)
        {
            Cell neighborCell = GetCellAtRelativePos(nodeIndex, curDirection);
            if (neighborCell != null)
            {
                neighbors.Add(neighborCell);
            }
        }

        return neighbors;

    }

    private Cell GetCellAtRelativePos(Vector2Int originPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = originPos + relativePos;

        if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
        {
            return null;
        } else
        {
            return cells[finalPos.x, finalPos.y];
        }


    }

    public Cell getCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (gridSize.x * cellDiameter);
        float percentY = worldPos.y / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt((gridSize.x) * percentX);
        int y = Mathf.FloorToInt((gridSize.y) * percentY);

        return cells[x, y];

    }



}

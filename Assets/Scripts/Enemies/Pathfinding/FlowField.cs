using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class FlowField
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Cell[,] cells;
    public Vector2Int gridSize;
    public float cellRadius;

    public Vector3 gridOrigin = Vector3.zero;
    public float cellDiameter;
    public Cell destinationCell;
    private Vector2 gridOffset = new Vector2(0.5f,0.5f);
    //public NativeArray<BoxcastCommand> commands = new NativeArray<BoxcastCommand>(1, Allocator.TempJob); //causes leak if uncommented
    QueryParameters qp = QueryParameters.Default;
    

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

    public void CreateGridOnPlayer()
    {
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        gridOrigin = new Vector3(
            playerPos.x - gridOffset.x - (gridSize.x * cellDiameter) / 2f,
            playerPos.y - gridOffset.y - (gridSize.y * cellDiameter) / 2f,
            0
        );

        cells = new Cell[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPosition = gridOrigin + new Vector3(
                    x * cellDiameter + cellRadius,
                    y * cellDiameter + cellRadius,
                    0
                );
                cells[x, y] = new Cell(worldPosition, new Vector2Int(x, y));
            }
        }
    }

    public void CreateCostField()
    {

        //Debug.Log("Creating cost");

        Vector3 cellHalf = Vector3.one * cellRadius;
        int terrainLayer = LayerMask.GetMask("Obstacle");
        foreach (Cell cell in cells)
        {
            //qp.layerMask = terrainLayer;
            //commands[0] = new BoxcastCommand(cell.worldPos, cellHalf, Quaternion.identity, Vector3.down, qp, 0f);



            Collider2D hit = Physics2D.OverlapBox(cell.worldPos, cellHalf, 0, terrainLayer);
            if (hit != null)
            {
                //Debug.Log("Hit " + hit.gameObject.name + " on layer " + LayerMask.LayerToName(hit.gameObject.layer));
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

        //var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
        //var handle = BoxcastCommand.ScheduleBatch(commands, results, 1, 9999, default);

        //handle.Complete();

        //foreach (RaycastHit hit in results)
        //{
        //    if (!hit.collider)
        //        continue;
        //    switch (hit.collider.gameObject.layer)
        //    {
        //        case 8: // Obstacle
        //            getCellFromWorldPos(hit.point).increaseCost(int.MaxValue);
        //            break;
        //        case 6: // Terrain
        //            getCellFromWorldPos(hit.point).increaseCost(0);
        //            break;
        //    }
        //}

        //results.Dispose();
        //commands.Dispose();


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

        //loop thru obstacle cells and increase cost of neighbors to deter ai from walking into them



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

    //causes infinite loop for some reason
    public void IncreaseCostAroundCells(Cell givenCell)
    {
        List<GridDirection> directionToCheck = GridDirection.AllDirections;

        //increase cost of neighboring cells
        foreach (GridDirection direction in directionToCheck)
        {
            Cell neighborCell = GetCellAtRelativePos(givenCell.gridIndex, direction);
            if (neighborCell != null)
            {
                neighborCell.increaseCost(5);
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
        // Offset by grid origin
        Vector3 localPos = worldPos - gridOrigin;

        float percentX = localPos.x / (gridSize.x * cellDiameter);
        float percentY = localPos.y / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSize.x * percentX), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSize.y * percentY), 0, gridSize.y - 1);

        return cells[x, y];
    }



}

using UnityEngine;

public class Cell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public int cost;
    public ushort bestCost;
    public GridDirection bestDirection;

    public Cell(Vector3 worldPos, Vector2Int gridIndex)
    {
        this.worldPos = worldPos;
        this.gridIndex = gridIndex;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void increaseCost(int costToAdd)
    {
        if (costToAdd == int.MaxValue)
        {
            cost = int.MaxValue;
            return;
        }

        if (cost + costToAdd >= int.MaxValue)
        {
            cost = int.MaxValue;
            return;
        }

        cost += costToAdd;

    }


}

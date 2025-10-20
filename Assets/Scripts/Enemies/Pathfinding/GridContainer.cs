using UnityEngine;

public class GridContainer : MonoBehaviour
{

    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField flowField;
    public bool displayGrid;
    public bool followingPlayer = true;

    private void InitField()
    {
        flowField = new FlowField(cellRadius, gridSize);
        flowField.CreateGrid();
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    InitField();
        //    flowField.CreateCostField();

        //    Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //    //Cell destinationCell = flowField.getCellFromWorldPos(worldPos);
        //    Cell destinationCell = flowField.getCellFromWorldPos(PlayerMovement.Instance.gameObject.transform.position);
        //    flowField.CreateIntegrationField(destinationCell);

        //    flowField.CreateFlowField();

        //}

        if (followingPlayer)
        {
            InitField();
            flowField.CreateCostField();

            Cell destinationCell = flowField.getCellFromWorldPos(PlayerMovement.Instance.gameObject.transform.position);
            flowField.CreateIntegrationField(destinationCell);

            flowField.CreateFlowField();
        }




    }

    private void OnDrawGizmos()
    {
        if (!displayGrid)
            return;

        if (flowField == null)
        {
            DrawGrid(gridSize, Color.white, cellRadius);

        } else
        {
            DrawGrid(flowField.gridSize, Color.blue, flowField.cellRadius);
        }



    }

    //draws the grid when gizmos drawing enabled
    private void DrawGrid(Vector2Int gridSize, Color drawColor, float drawCellRadius)
    {
        Gizmos.color = drawColor;
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Vector3 cellPosition = new Vector3(drawCellRadius * 2 * i + drawCellRadius, drawCellRadius * 2 * j + drawCellRadius, 0);
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(cellPosition, size);
                //now draw cell cost
                if (flowField != null)
                {
                    Cell cell = flowField.cells[i, j];
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    UnityEditor.Handles.Label(cell.worldPos, cell.cost.ToString(), style);

                }
                }
            }


    }
}

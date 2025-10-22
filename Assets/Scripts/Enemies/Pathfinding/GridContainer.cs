using UnityEngine;

public class GridContainer : MonoBehaviour
{

    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField flowField;
    public bool displayGrid;
    public bool followingPlayer = true;
    public bool centerOnPlayer = true;


    private void InitField()
    {
        Camera mainCamera = Camera.main;

        float halfHeight = mainCamera.orthographicSize;

        // Calculate half the camera's horizontal view size in world units
        float halfWidth = halfHeight * mainCamera.aspect;

        // Calculate the camera's position
        Vector3 cameraPos = mainCamera.transform.position;

        // Calculate the world coordinates of the camera's bounds
        float leftBound = cameraPos.x - halfWidth;
        float rightBound = cameraPos.x + halfWidth;
        float bottomBound = cameraPos.y - halfHeight;
        float topBound = cameraPos.y + halfHeight;

        Debug.Log("Camera Bounds: Left=" + leftBound + ", Right=" + rightBound + ", Bottom=" + bottomBound + ", Top=" + topBound);

        gridSize.x = Mathf.CeilToInt((rightBound - leftBound) / (cellRadius * 2));
        gridSize.y = Mathf.CeilToInt((topBound - bottomBound) / (cellRadius * 2));

        flowField = new FlowField(cellRadius, gridSize);

        if (centerOnPlayer)
        {
            Debug.Log("Creating grid on player");
            flowField.CreateGridOnPlayer();
        }
        else
        {
            flowField.CreateGrid();
        }
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
                Vector3 cellPosition = new Vector3(flowField.cells[i, j].worldPos.x, flowField.cells[i, j].worldPos.y, 0);

                //Vector3 cellPosition = new Vector3(drawCellRadius * 2 * i + drawCellRadius, drawCellRadius * 2 * j + drawCellRadius, 0);
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(cellPosition, size);
                //now draw cell cost
                if (flowField != null)
                {
                    Cell cell = flowField.cells[i, j];
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    UnityEditor.Handles.Label(cell.worldPos, cell.bestCost.ToString(), style);

                }
                }
            }


    }
}

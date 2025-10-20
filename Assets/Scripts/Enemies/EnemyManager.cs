using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GridContainer gridContainer;
    public GameObject enemyPrefab;
    public int UnitsPerSpawn = 2;

    private List<GameObject> enemiesSpawned;

    private void Awake()
    {
        enemiesSpawned = new List<GameObject>();   
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemies();
        }

    }

    private void FixedUpdate()
    {
        if (gridContainer.flowField == null)
            return;

        foreach (GameObject enemy in enemiesSpawned)
        {
            Cell cellBelow = gridContainer.flowField.getCellFromWorldPos(enemy.transform.position);
            Vector3 flowDirection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y);
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>(); //calling getcomponent a lot, very expensive..
            enemyRb.linearVelocity = flowDirection.normalized * 2f;
        }

    }

    private void SpawnEnemies()
    {
        Vector2Int gridSize = gridContainer.gridSize;
        float cellRadius = gridContainer.cellRadius;
        GameObject enemyInstance = Instantiate(enemyPrefab);
        enemyInstance.transform.parent = transform;
        enemiesSpawned.Add(enemyInstance);
        enemyInstance.transform.position = new Vector3(0, 0, 0);
    }

}

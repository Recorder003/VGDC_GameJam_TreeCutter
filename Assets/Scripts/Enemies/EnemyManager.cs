using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GridContainer gridContainer;
    public GameObject enemyPrefab;
    public int UnitsPerSpawn = 2;

    private List<EnemyObj> enemiesSpawned;

    class EnemyObj
    {
        public EnemyBase enemyBase;
        public GameObject obj;
        public Rigidbody2D enemyRb;
        public EnemyObj(GameObject enemy)
        {
            enemyBase = enemy.GetComponent<EnemyBase>();
            enemyRb = enemy.GetComponent<Rigidbody2D>();
            obj = enemy;
        }
    }

        private void Awake()
    {
        enemiesSpawned = new List<EnemyObj>();   
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

        foreach (EnemyObj enemy in enemiesSpawned)
        {
            Cell cellBelow = gridContainer.flowField.getCellFromWorldPos(enemy.obj.transform.position);
            Vector3 flowDirection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y);
            Rigidbody2D enemyRb = enemy.enemyRb; 
            enemyRb.linearVelocity = flowDirection.normalized * 2f;
        }

    }

    private void SpawnEnemies()
    {
        Vector2Int gridSize = gridContainer.gridSize;
        float cellRadius = gridContainer.cellRadius;
        GameObject enemyInstance = Instantiate(enemyPrefab);
        enemyInstance.transform.parent = transform;
        enemiesSpawned.Add(new EnemyObj(enemyInstance));
        enemyInstance.transform.position = new Vector3(0, 0, 0);
    }

}

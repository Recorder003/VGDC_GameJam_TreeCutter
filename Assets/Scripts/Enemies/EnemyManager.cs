using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool debug = true;

    public GridContainer gridContainer;
    public GameObject enemyPrefab;
    public List<GameObject> enemyPrefabs;
    //public int UnitsPerSpawn = 2;
    private int maxEnemies = 30;

    public string testEnemyToSpawn;
    private List<EnemyObj> enemiesSpawned;

    public static EnemyManager Instance { get; private set; }
    public Vector3 playerPos;
    public int currentDifficulty; //set to public for testing
    public bool spawningEnemies = false;


    class EnemyObj
    {
        public EnemyBase enemyBase;
        public GameObject obj;
        public Rigidbody2D enemyRb;
        public Collider2D enemyCollider;
        public bool outsideGrid = false;
        public EnemyObj(GameObject enemy)
        {
            enemyBase = enemy.GetComponent<EnemyBase>();
            enemyRb = enemy.GetComponent<Rigidbody2D>();
            obj = enemy;
            enemyCollider = enemy.GetComponent<Collider2D>();
        }
    }

    private void Awake()
    {
        enemiesSpawned = new List<EnemyObj>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentDifficulty = 0;

    }

    // Update is called once per frame
    void Update()
    {
        playerPos = PlayerMovement.Instance.transform.position;
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SpawnEnemyFromName(testEnemyToSpawn);
            }
        }


        currentDifficulty = Mathf.FloorToInt(Time.timeSinceLevelLoad / 3f);


    }

    private void FixedUpdate()
    {
        if (gridContainer.flowField == null)
            return;

        if (enemiesSpawned.Count < maxEnemies && spawningEnemies)
        {
            float roll = Random.Range(0f, 1f);
            if (roll < 0.05f)
            {
                chooseEnemy();
            }
        }

        foreach (EnemyObj enemy in enemiesSpawned)
        {
            if (enemy.enemyBase.movementType == EnemyBase.MovementType.Normal)
            {
                Cell cellBelow = gridContainer.flowField.getCellFromWorldPos(enemy.obj.transform.position);
                Vector3 flowDirection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y);
                Rigidbody2D enemyRb = enemy.enemyRb; //calling getcomponent a lot, very expensive..
                enemyRb.linearVelocity = flowDirection.normalized * enemy.enemyBase.speed;
            }
            else if (enemy.enemyBase.movementType == EnemyBase.MovementType.Flying)
            {
                enemy.obj.transform.position = Vector2.MoveTowards(transform.position, playerPos, enemy.enemyBase.speed * Time.deltaTime); //doesnt work
            }
            else if (enemy.enemyBase.movementType == EnemyBase.MovementType.Ghost)
            {
                Rigidbody2D enemyRb = enemy.enemyRb;
                enemyRb.linearVelocity = (playerPos - enemy.obj.transform.position).normalized * enemy.enemyBase.speed;
            }


            //if enemy is outside grid, turn off colliding with obstacles until back in grid, skip if already avoiding obstacles
            if (enemy.enemyBase.movementType == EnemyBase.MovementType.Flying || enemy.enemyBase.movementType == EnemyBase.MovementType.Ghost)
                continue;

            Vector2Int gridSize = gridContainer.flowField.gridSize;
            Cell cellCheck = gridContainer.flowField.getCellFromWorldPos(enemy.obj.transform.position);
            //print("Enemy " + enemy.obj.name + " at grid index " + cellCheck.gridIndex);
            if (cellCheck.gridIndex.x + 1 == gridSize.x || cellCheck.gridIndex.x == 0 || cellCheck.gridIndex.y + 1 == gridSize.y || cellCheck.gridIndex.y == 0)
            {
                enemy.outsideGrid = true;
            }
            else
            {
                enemy.outsideGrid = false;
            }


            if (enemy.outsideGrid)
            {
                enemy.enemyCollider.excludeLayers = LayerMask.GetMask("Obstacle");
            }
            else
            {
                enemy.enemyCollider.excludeLayers = 0;
            }


            if (enemy.obj.transform.position.x > playerPos.x)
            {
                enemy.enemyBase.spriteRenderer.flipX = true;
            }
            else
            {
                enemy.enemyBase.spriteRenderer.flipX = false;
            }

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

    private void SpawnEnemyFromName(string enemyName)
    {
        GameObject enemyPrefab = enemyPrefabs.Find(e => e.name == enemyName);
        GameObject enemyInstance = Instantiate(enemyPrefab);
        enemyInstance.name = enemyPrefab.name + "_" + Time.timeSinceLevelLoad.ToString("F2");
        enemyInstance.transform.parent = transform;
        Vector3 spawnPos = Vector3.zero;

        Vector2Int gridSize = gridContainer.flowField.gridSize;
        //print("Grid Size: " + gridSize);

        int randomX = Random.Range(0, gridSize.x-1);
        int randomY = Random.Range(0, gridSize.y - 1);

        //spawnPos = gridContainer.flowField.cells[randomX, randomY].worldPos;
        

        int randomEdge = Random.Range(0, 4);

        switch (randomEdge)
        {
            case 0:
                //top
                spawnPos = gridContainer.flowField.cells[randomX, gridSize.y - 1].worldPos + Vector3.up * 5; //add to the y so spawns outside of camera
                break;
            case 1:
                //bottom
                spawnPos = gridContainer.flowField.cells[randomX, 0].worldPos + Vector3.down * 5;
                break;
            case 2:
                //left
                spawnPos = gridContainer.flowField.cells[0, randomY].worldPos + Vector3.left * 5;
                break;
            case 3:
                //right
                spawnPos = gridContainer.flowField.cells[gridSize.x - 1, randomY].worldPos + Vector3.right * 5;
                break;

        }

        enemyInstance.transform.position = spawnPos;
        enemiesSpawned.Add(new EnemyObj(enemyInstance));

        //top row = max y, bottom row = min y, left col = min x, right col = max x
    }

    private void chooseEnemy()
    {
        //idea is to have enemies closer to the difficulty level to have a higher chance to spawn, but higher and lower difficulties to have a lower chance

        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            EnemyBase enemyBase = enemyPrefab.GetComponent<EnemyBase>();
            int difficultyDiff = Mathf.Abs(enemyBase.difficultyLevel - currentDifficulty);
            float spawnChance = 1f / (difficultyDiff + 1); //the closer the difficulty, the higher the chance
            float roll = Random.Range(0f, 1f);
            if (roll < spawnChance)
            {
                SpawnEnemyFromName(enemyPrefab.name);
                return;
            }
        }
    }

        public void EnemyKilled(GameObject enemyObj)
    {
        enemiesSpawned.RemoveAll(e => e.obj == enemyObj);
    }

}

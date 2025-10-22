using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyData.MoveSpeed * Time.deltaTime); //constantly move enemy towards player
    }
}

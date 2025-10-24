using UnityEngine;

public class PineTree : EnemyBase
{
    private Transform player;

    public GameObject bulletPrefab;

    private float shotCooldown;

    public float startShotCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shotCooldown = startShotCooldown;
    }

    void Update()
    {
        Vector2 direction = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);

        if (shotCooldown<= 0){
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Pine>().SetDirection(direction);

            shotCooldown = startShotCooldown;
        }
        else
        {
            shotCooldown-=Time.deltaTime;
        }
    }

}

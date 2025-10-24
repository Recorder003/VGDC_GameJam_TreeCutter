using UnityEngine;

public class Pine : EnemyProjectile
{



    void Start()
    {
        speed = 5f;
        lifeTime = 3f;
        damage = 5;
        Destroy(gameObject, lifeTime); //might error if object already destroyed

    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }



    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }


}

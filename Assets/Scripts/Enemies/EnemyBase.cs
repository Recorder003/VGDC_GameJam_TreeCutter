using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int health;
    public int speed;
    public int collideDamage;
    // could have some sort of unique pathing for certain enemies

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            // TODO: Damage player
            return;
        }

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        throw new NotImplementedException();
    }
}

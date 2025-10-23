using DG.Tweening;
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public enum MovementType
    {
        Normal, //uses pathing
        Flying, //avoids obstacles, collides with other flyers
        Ghost //ignores all collisions
    }

    public MovementType movementType;
    public int health;
    public int speed;
    public int collideDamage;
    private SpriteRenderer spriteRenderer;
    private float flashDuration;
    public string hitSfxName;
    // could have some sort of unique pathing for certain enemies

    void Start()
    {
        flashDuration = GameManager.Instance.enemyHitFlashDur;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void TakeDamage(int damage)
    {
        health -= damage;
        DOTween.Complete(spriteRenderer);
        SoundManager.Instance.PlaySFXRandomPitch("enemyHurt");


        if (health <= 0)
        {
            Die();
        } else
        {
            gameObject.transform.DOShakeRotation(0.2f, 10, 20, 90, false);
            spriteRenderer.color = Color.red;
            spriteRenderer.DOColor(Color.white, flashDuration);
            //play hit sfx
        }
    }



    private void Die()
    {
        //might want to store any coroutines happening on enemy and end them
        //throw new NotImplementedException();
        EnemyManager.Instance.EnemyKilled(gameObject);
        Destroy(gameObject);
    }
}

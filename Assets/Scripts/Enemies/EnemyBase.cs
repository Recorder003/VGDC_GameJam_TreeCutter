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
    public SpriteRenderer spriteRenderer;
    private float flashDuration;
    public string hitSfxName;
    public int difficultyLevel;
    // could have some sort of unique pathing for certain enemies

    private void Awake()
    {
        flashDuration = GameManager.Instance.enemyHitFlashDur;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //flashDuration = GameManager.Instance.enemyHitFlashDur;
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //check if spriterender is null


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
            if (spriteRenderer != null)
            {

                spriteRenderer.color = Color.red;
                spriteRenderer.DOColor(Color.white, flashDuration);
            }
            else
            {
                Debug.LogWarning("SpriteRenderer is null on enemy: " + gameObject.name);
            }

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

using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    //Current stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    private SpriteRenderer spriteRenderer;

    public static event Action<int> LeveledUp;

    //Experience and level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    //Class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;


    void Awake()
    {
        //Assign the variables
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
    }


    void Start()
    {
        //Initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        //If the invincibility timer has reached 0, set the invincibility flag to false
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            GameManager.Instance.playerLeveledUp(level);
        }
    }

    public void TakeDamage(float dmg)
    {
        //If the player is not currently invincible, reduce health and start invinciblity
        if (!isInvincible)
        {
            currentHealth -= dmg;

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD");
    }

    public void RestoreHealth(float amount)
    {
        //Only heal player if current health is less than max health
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += amount;

            //Make sure player's health doesn't exceed max health
            if (currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Triggered with " + collision.gameObject.name);

        var enemyTag = collision.gameObject.tag;

        switch (enemyTag)
        {
            case "Enemy":
                TakeDamage(collision.gameObject.GetComponent<EnemyBase>().collideDamage);
                break;

        }

    }

    private void TakeDamage(int damage)
    {

        if (isInvincible)
            return;

        print("Player takes " + damage + " damage.");
        print("Player health before damage: " + currentHealth);
        currentHealth -= damage;
        //start hurt flash animation

        if (currentHealth <= 0)
        {
            PlayerDies();
        }
        else
        {
            HurtFlash();
            StartCoroutine(IFramesCoro());
        }
    }

    private void HurtFlash()
    {
        //implement hurt flash animation here
        spriteRenderer.color = Color.red;
        spriteRenderer.DOColor(Color.white, invincibilityTimer);

    }

    private void PlayerDies()
    {
        print("Player Died");
        //bring up death ui
    }



    private IEnumerator IFramesCoro()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTimer);
        isInvincible = false;
    }

}

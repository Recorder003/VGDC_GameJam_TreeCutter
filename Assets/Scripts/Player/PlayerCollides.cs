using System.Collections;
using UnityEngine;
using DG.Tweening;
//using UnityEditor.U2D.Animation;

public class PlayerCollides : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int maxHealth;
    public bool invulnerable = false;
    public float iFrameDuration = 0.2f;
    private SpriteRenderer spriteRenderer;


    public static PlayerCollides Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        maxHealth = 50;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        if (invulnerable)
            return;

        print("Player takes " + damage + " damage.");
        print("Player health before damage: " + maxHealth);
        maxHealth -= damage;
        //start hurt flash animation

        if (maxHealth <= 0)
        {
            PlayerDies();
        } else
        {
            HurtFlash();
            StartCoroutine(IFramesCoro());
        }
    }

    private void HurtFlash()
    {
        //implement hurt flash animation here
        spriteRenderer.color = Color.red;
        spriteRenderer.DOColor(Color.white, iFrameDuration);

    }

    private void PlayerDies()
    {
        print("Player Died");
        //bring up death ui
    }



    private IEnumerator IFramesCoro()
    {
        invulnerable = true;
        yield return new WaitForSeconds(iFrameDuration);
        invulnerable = false;
    }


}

using UnityEngine;

// Base script for all weapon controllers

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    public float currentCooldown;

    protected PlayerMovement pm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        weaponData = Instantiate(weaponData); // Create a new instance of the weapon data so that each weapon controller has its own copy
        pm = FindObjectOfType<PlayerMovement>();
        currentCooldown = weaponData.CooldownDuration;// At the start set the current cooldown to be the cooldown duration
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}

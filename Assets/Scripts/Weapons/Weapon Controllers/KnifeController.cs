using UnityEngine;

public class KnifeController : WeaponController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static KnifeController Instance { get; private set; }


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

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        spawnedKnife.transform.position=transform.position;// Assign the position to be the same as this object which is parented to the player
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector); //Reference and set the direction
    }

    public void decreaseCooldown(float num)
    {
        if (weaponData.CooldownDuration - num >= 0.1f)
        {
            weaponData.CooldownDuration -= num;
        }
        else
        {
            weaponData.CooldownDuration = 0.1f;
        }
    }

}
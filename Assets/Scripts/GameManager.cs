using UnityEngine;

public class GameManager : MonoBehaviour
{
    // General game manager, holds stats other scripts can reference through GameManager.Instance.value

    public int playerLives = 3;
    public float gameSpeed = 1f;
    public float musicVolume = 0.5f;
    public float enemyHitFlashDur = 0.2f;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        //set this to instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


}

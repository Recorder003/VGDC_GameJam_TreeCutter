using UnityEngine;

public class AxeMulti : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    GameObject KnifeControllerPrefab;
    private void Awake()
    {
        Instantiate(KnifeControllerPrefab, PlayerStats.Instance.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

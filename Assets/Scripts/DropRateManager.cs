using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;

    //void OnDestroy()
    //{
    //    if (!SceneManager.GetActiveScene().isLoaded) //doesnt work
    //    {
    //        float randomNumber = UnityEngine.Random.Range(0f, 100f);
    //        List<Drops> possibleDrops = new List<Drops>();

    //        foreach (Drops rate in drops)
    //        {
    //            if (randomNumber <= rate.dropRate)
    //            {
    //                possibleDrops.Add(rate);
    //            }
    //        }

    //        //Check if there are possible drops
    //        if (possibleDrops.Count > 0)
    //        {
    //            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];

    //            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity); //this function is called when game closes, so instantiates items even after game is over, memory leak?
    //        }
    //    }


    //}

    public void itemDestroyed()
    {
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }

        //Check if there are possible drops
        if (possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];

            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity); //this function is called when game closes, so instantiates items even after game is over, memory leak?
        }
    }

}

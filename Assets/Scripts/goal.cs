using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal : MonoBehaviour
{
    public DungeonMaker dMaker;

    private void Start()
    {
        dMaker = FindObjectOfType<DungeonMaker>();
        Debug.Log("AA");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dMaker.ResetGame();
        }
    }
}

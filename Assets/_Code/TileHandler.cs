using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    EndlessTileSpawner endlessTileSpawner;

    private void Start()
    {        
        endlessTileSpawner = GameObject.FindObjectOfType<EndlessTileSpawner>();
    }

    private void OnTriggerExit(Collider other)
    {
        endlessTileSpawner.SpawnTile();
        Destroy(gameObject, 15);
    }
}

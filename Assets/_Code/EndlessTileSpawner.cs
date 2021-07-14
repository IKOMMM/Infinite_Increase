using UnityEngine;

public class EndlessTileSpawner : MonoBehaviour
{
    public GameObject[] groundTiles;
    Vector3 nextSpawnPoint;

    private int randomTilePrefab;

    public void SpawnTile()
    {
        randomTilePrefab = Random.Range(0, groundTiles.Length);

        GameObject tempTile = Instantiate(groundTiles[randomTilePrefab], nextSpawnPoint, Quaternion.identity);

        nextSpawnPoint = tempTile.transform.GetChild(1).transform.position;
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnTile();
        }
        
    }

}

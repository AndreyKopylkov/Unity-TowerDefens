using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyFactory OriginFactory { get; set; }

    public void SpawnOn(GameTile tile)
    {
        transform.localPosition = tile.transform.localPosition;
    }
}
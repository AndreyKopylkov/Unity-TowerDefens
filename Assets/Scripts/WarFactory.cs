using UnityEngine;

[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{
    public void Reclaim(WarEntity entity)
    {
        Destroy(entity.gameObject);
    }
}
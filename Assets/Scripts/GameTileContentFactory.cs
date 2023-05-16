using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField] private GameTileContent _destinationPrefab;
    [SerializeField] private GameTileContent _emptyPrefab;
    [SerializeField] private GameTileContent _wallPrefab;
    [SerializeField] private GameTileContent _spawnPointPrefab;
    
    public void Reclaim(GameTileContent content)
    {
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentsType type)
    {
        switch (type)
        {
            case GameTileContentsType.Destination:
                return Get(_destinationPrefab);
            case GameTileContentsType.Empty:
                return Get(_emptyPrefab);
            case GameTileContentsType.Wall:
                return Get(_wallPrefab);
            case GameTileContentsType.SpawnPoint:
                return Get(_spawnPointPrefab);
        }

        return null;
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
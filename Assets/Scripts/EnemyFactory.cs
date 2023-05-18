using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField, FloatRangeSlider(0.5f, 2f)] private FloatRange _scale = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)] private FloatRange _pathOffset = new FloatRange(0f);
    [SerializeField, FloatRangeSlider(0.2f, 5f)] private FloatRange _speed = new FloatRange(1f);

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(_enemyPrefab);
        instance.OriginFactory = this;
        instance.Initialize(_scale.RandomValue, _pathOffset.RandomValue, _speed.RandomValue);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
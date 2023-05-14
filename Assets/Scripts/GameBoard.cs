using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;

    private Vector2Int _size;
    private GameTile[] _tiles;
    private Queue<GameTile> _searchFrontierTiles = new Queue<GameTile>();

    public void Initialize(Vector2Int size)
    {
        _size = size;
        _ground.localScale = new Vector3(_size.x, _size.y, 1f);

        _tiles = new GameTile[_size.x * _size.y];
        
        Vector2 offset = new Vector2((_size.x - 1) * 0.5f, (_size.y - 1) * 0.5f);
        for (int i = 0, y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++, i++)
            {
                GameTile newTile = Instantiate(_tilePrefab);
                _tiles[i] = newTile;
                newTile.transform.SetParent(transform, false);
                newTile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if (x > 0)
                    GameTile.MakeEastWestTileNeighbors(newTile, _tiles[i - 1]);
                if (y > 0)
                    GameTile.MakeNorthSouthTileNeighbors(newTile, _tiles[i - _size.x]);

                newTile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                    newTile.IsAlternative = !newTile.IsAlternative;
            }
        }

#if UNITY_EDITOR
        FindPaths();
#endif
    }

    public void FindPaths()
    {
        foreach (var tile in _tiles)
        {
            tile.ClearPath();
        }

        int destinationIndex = _tiles.Length / 2;
        _tiles[destinationIndex].BecomeDestination();
        _searchFrontierTiles.Enqueue(_tiles[destinationIndex]);

        while (_searchFrontierTiles.Count > 0)
        {
            GameTile tile = _searchFrontierTiles.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontierTiles.Enqueue(tile.GrowPathNorth());
                    _searchFrontierTiles.Enqueue(tile.GrowPathSouth());
                    _searchFrontierTiles.Enqueue(tile.GrowPathEast());
                    _searchFrontierTiles.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    _searchFrontierTiles.Enqueue(tile.GrowPathWest());
                    _searchFrontierTiles.Enqueue(tile.GrowPathEast());
                    _searchFrontierTiles.Enqueue(tile.GrowPathSouth());
                    _searchFrontierTiles.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (var tile in _tiles)
        {
            tile.ShowPath();
        }
    }
}

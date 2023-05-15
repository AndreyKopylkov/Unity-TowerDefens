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
    private GameTileContentFactory _gameTileContentFactory;

    public void Initialize(Vector2Int size, GameTileContentFactory gameTileContentFactory)
    {
        _size = size;
        _ground.localScale = new Vector3(_size.x, _size.y, 1f);

        _tiles = new GameTile[_size.x * _size.y];
        _gameTileContentFactory = gameTileContentFactory;
        
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

                newTile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Empty);
            }
        }

        ToggleDestination(_tiles[_tiles.Length / 2]);
    }

    public bool FindPaths()
    {
        // foreach (var tile in _tiles)
        // {
        //     tile.ClearPath();
        // }
        //
        // int destinationIndex = _tiles.Length / 2;
        // _tiles[destinationIndex].BecomeDestination();
        // _searchFrontierTiles.Enqueue(_tiles[destinationIndex]);

        foreach (var tile in _tiles)
        {
            if (tile.GameTileContent.GameTileContentType == GameTileContentsType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontierTiles.Enqueue(tile);
                Debug.Log(_searchFrontierTiles);
            }
            else
            {
                tile.ClearPath();
            }
        }
        
        if(_searchFrontierTiles.Count == 0)
            return false;

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
            if (!tile.HasPath)
                return false;
        }

        foreach (var tile in _tiles)
        {
            tile.ShowPath();
        }

        return true;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.GameTileContent.GameTileContentType == GameTileContentsType.Destination)
        {
            tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Empty);
            if (!FindPaths())
            {
                tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Destination);
            }
        }
        else if(tile.GameTileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Destination);
        }
        
        FindPaths();
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.GameTileContent.GameTileContentType == GameTileContentsType.Wall)
        {
            tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Empty);
        }
        else if(tile.GameTileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Wall);
            if (!FindPaths())
            {
                tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Empty);
            }
        }
        FindPaths();
    }

    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            int x =  (int) (hit.point.x + _size.x * 0.5f);
            int y =  (int) (hit.point.z + _size.y * 0.5f);
            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _tiles[x + y * _size.x];
            }
        }

        return null;
    }
}

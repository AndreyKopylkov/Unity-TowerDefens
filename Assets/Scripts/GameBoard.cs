using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;
    [SerializeField] private LayerMask _tileLayerMask;
    [SerializeField] private LevelInfo _levelInfo;
    [SerializeField] private LevelStartTypes _levelStartType = LevelStartTypes.EditInRuntime;

    private Vector2Int _size;
    private GameTile[] _tiles;
    private Queue<GameTile> _searchFrontierTiles = new Queue<GameTile>();
    private GameTileContentFactory _tileContentFactory;
    private List<GameTile> _spawnPointsTiles = new List<GameTile>();
    private List<GameTileContent> _contentsToUpdate = new List<GameTileContent>();

    public int SpawnPointCount => _spawnPointsTiles.Count;
    
    public enum LevelStartTypes { EditInRuntime, LevelInfo }

    public void Initialize(Vector2Int size, GameTileContentFactory gameTileContentFactory)
    {
        _size = size;
        _ground.localScale = new Vector3(_size.x, _size.y, 1f);

        _tiles = new GameTile[_size.x * _size.y];
        _tileContentFactory = gameTileContentFactory;
        
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
        
        Clear();
    }

    public void GameUpdate()
    {
        foreach (var content in _contentsToUpdate)
        {
            content.GameUpdate();
        }
    }

    public bool FindPaths()
    {
        foreach (var tile in _tiles)
        {
            if (tile.TileContent.GameTileContentType == GameTileContentsType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontierTiles.Enqueue(tile);
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
        if (tile.TileContent.GameTileContentType == GameTileContentsType.Destination)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
            if (!FindPaths())
            {
                tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Destination);
            }
        }
        else if(tile.TileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Destination);
        }
        
        FindPaths();
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.TileContent.GameTileContentType == GameTileContentsType.Wall)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
        }
        else if(tile.TileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Wall);
            if (!FindPaths())
            {
                tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
            }
        }
        FindPaths();
    }
    
    public void ToggleTower(GameTile tile)
    {
        if (tile.TileContent.GameTileContentType == GameTileContentsType.Tower)
        {
            _contentsToUpdate.Remove(tile.TileContent);
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
        }
        else if(tile.TileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Tower);
            if (FindPaths())
            {
                _contentsToUpdate.Add(tile.TileContent);
            }
            else
            {
                tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
            }
        }
        if (tile.TileContent.GameTileContentType == GameTileContentsType.Wall)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Tower);
            _contentsToUpdate.Add(tile.TileContent);
        }
        FindPaths();
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.TileContent.GameTileContentType == GameTileContentsType.SpawnPoint)
        {
            if (_spawnPointsTiles.Count > 1)
            {
                _spawnPointsTiles.Remove(tile);
                tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
            }
        }
        else if(tile.TileContent.GameTileContentType == GameTileContentsType.Empty)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.SpawnPoint);
            _spawnPointsTiles.Add(tile);
        }
    }

    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, _tileLayerMask))
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

    public GameTile GetSpawnPoint(int index)
    {
        return _spawnPointsTiles[index];
    }

    public void Clear()
    {
        foreach (var tile in _tiles)
        {
            tile.TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
        }
        
        _spawnPointsTiles.Clear();
        _contentsToUpdate.Clear();

        switch (_levelStartType)
        {
            case LevelStartTypes.EditInRuntime:
                ToggleDestination(_tiles[_tiles.Length / 2]);
                ToggleSpawnPoint(_tiles[0]);
                break;
            case LevelStartTypes.LevelInfo:
                CreateLevel();
                FindPaths();
                break;
        }
    }

    //{ Empty, Destination, Wall, SpawnPoint, Tower}
    private void CreateLevel()
    {
        for (int i = 0, x = 0; i < _levelInfo.columns.Length; i++)
        {
            for (int j = 0; j < _levelInfo.columns[0].rows.Length; j++, x++)
            {
                var type = _levelInfo.columns[i].rows[j];
                switch (type)
                {
                    case 0:
                        _tiles[x].TileContent = _tileContentFactory.Get(GameTileContentsType.Empty);
                        break;
                    case 1:
                        _tiles[x].TileContent = _tileContentFactory.Get(GameTileContentsType.Destination);
                        break;
                    case 2:
                        _tiles[x].TileContent = _tileContentFactory.Get(GameTileContentsType.Wall);
                        break;
                    case 3:
                        _tiles[x].TileContent = _tileContentFactory.Get(GameTileContentsType.SpawnPoint);
                        _spawnPointsTiles.Add(_tiles[x]);
                        break;
                    case 4:
                        _tiles[x].TileContent = _tileContentFactory.Get(GameTileContentsType.Tower);
                        _contentsToUpdate.Add(_tiles[x].TileContent);
                        break;
                }
            }
        }
    }
}
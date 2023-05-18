using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Transform _arrow;
    
    public bool IsAlternative { get; set; }
    public bool HasPath => _distance != int.MaxValue;

    private GameTile _northTile, _eastTile, _southTile, _westTile;
    private GameTile _nextTileOnPath;
    private int _distance;
    private readonly Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private readonly Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 0f);
    private readonly Quaternion _southRotation = Quaternion.Euler(90f, 180f, 0f);
    private readonly Quaternion _westRotation = Quaternion.Euler(90f, 270f, 0f);
    private GameTileContent _tileContent;

    public GameTile NextTileOnPath => _nextTileOnPath;
    public Vector3 ExitPoint { get; private set; }
    public Direction PathDirection { get; private set; }

    public GameTileContent TileContent
    {
        get => _tileContent;
        set { if(_tileContent != null)
                _tileContent.Recycle();

            _tileContent = value;
            _tileContent.transform.localPosition = transform.localPosition;
        }
    }

    public static void MakeEastWestTileNeighbors(GameTile eastTile, GameTile westTile)
    {
        westTile._eastTile = eastTile;
        eastTile._westTile = westTile;
    }

    public static void MakeNorthSouthTileNeighbors(GameTile northTile, GameTile southTile)
    {
        northTile._southTile = southTile;
        southTile._northTile = northTile;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        _nextTileOnPath = null;
    }

    public void BecomeDestination()
    {
        _distance = 0;
        _nextTileOnPath = null;
        ExitPoint = transform.localPosition;
    }

    private GameTile GrowPathTo(GameTile neighbourTile, Direction direction)
    {
        if (!HasPath || neighbourTile == null || neighbourTile.HasPath)
            return null;

        neighbourTile._distance = _distance + 1;
        neighbourTile._nextTileOnPath = this;
        neighbourTile.ExitPoint = neighbourTile.transform.localPosition + direction.GetHalfVector();
        neighbourTile.PathDirection = direction;
        return neighbourTile._tileContent.IsBlockingPath ? null : neighbourTile;
    }

    public GameTile GrowPathNorth() => GrowPathTo(_northTile, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(_eastTile, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(_southTile, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(_westTile, Direction.East);

    public void ShowPath()
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            _nextTileOnPath == _northTile ? _northRotation :
            _nextTileOnPath == _eastTile ? _eastRotation :
            _nextTileOnPath == _southTile ? _southRotation : 
            _westRotation; 
    }
}
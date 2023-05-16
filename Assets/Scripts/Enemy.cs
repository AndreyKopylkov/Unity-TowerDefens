using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyFactory OriginFactory { get; set; }

    private GameTile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _progress;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _directionAngleFrom, _directionAngleTo;

    public void SpawnOn(GameTile tile)
    {
        transform.localPosition = tile.transform.localPosition;
        _tileFrom = tile;
        _tileTo = tile.NextTileOnPath;
        _progress = 0f;
        
        PrepareInfo();
    }

    private void PrepareInfo()
    {
        _positionFrom = _tileFrom.transform.localPosition;
        _positionTo = _tileFrom.ExitPoint;
        _direction = _tileFrom.PathDirection;
        _directionChange = DirectionChange.None;
        _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
        transform.localRotation = _direction.GetRotation();
    }

    public bool GameUpdate()
    {
        _progress += Time.deltaTime;
        while (_progress > 1f)
        {
            _tileFrom = _tileTo;
            _tileTo = _tileTo.NextTileOnPath;
            if (_tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            
            _progress -= 1f;
            PrepareNextState();
        }

        transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
        if (_directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    private void PrepareNextState()
    {
        _positionFrom = _positionTo;
        _positionTo = _tileFrom.ExitPoint;
        _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
        _direction = _tileFrom.PathDirection;
        _directionAngleFrom = _directionAngleTo;

        switch (_directionChange)
        {
            case DirectionChange.None:
                PrepareForward();
                Debug.Log("Forward");
                break;
            case DirectionChange.TurnRight:
                PrepareTurnRight();
                break;
            case DirectionChange.TurnLeft:
                PrepareTurnLeft();
                break;
            default:
                Debug.Log("Around");
                PrepareAround();
                break;
        }
    }

    private void PrepareForward()
    {
        transform.localRotation = _direction.GetRotation();
        _directionAngleTo = _direction.GetAngle();
    }
    private void PrepareTurnRight()
    {
        _directionAngleTo = _directionAngleFrom + 90f;
    }
    private void PrepareTurnLeft()
    {
        _directionAngleTo = _directionAngleFrom - 90f;
    }
    private void PrepareAround()
    {
        _directionAngleTo = _directionAngleFrom + 180f;
    }
}
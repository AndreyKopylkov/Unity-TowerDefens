﻿using UnityEngine;

public class Enemy : GameBehavior
{
    [SerializeField] private Transform _model;
    
    public EnemyFactory OriginFactory { get; set; }

    private GameTile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _progress, _progressFactor;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _directionAngleFrom, _directionAngleTo;
    private float _pathOffset;
    private float _speed;
    private int _moneyCost;
    private int _damageToBase;
    private bool _isAlive;
    
    public float Scale { get; private set; }
    public float Health { get; private set; }

    public void Initialize(float scale, float pathOffset, float speed, float health, int moneyCost, int damageToBase)
    {
        _model.localScale = new Vector3(scale, scale, scale);
        _pathOffset = pathOffset;
        _speed = speed;
        Scale = scale;
        Health = health;
        _moneyCost = moneyCost;
        _damageToBase = damageToBase;
        _isAlive = true;
    }
    
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
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localRotation = _direction.GetRotation();
        _progressFactor = 2f * _speed;
    }

    private void PrepareOutro()
    {
        _positionTo = _tileFrom.transform.localPosition;
        _directionChange = DirectionChange.None;
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localRotation = _direction.GetRotation();
        _progressFactor = 2f * _speed;
    }

    public override bool GameUpdate()
    {
        if (!_isAlive)
        {
            Death();
            Recycle();
            return false;
        }
            
        _progress += Time.deltaTime * _progressFactor;
        while (_progress > 1f)
        {
            if (_tileTo == null)
            {
                Game.EnemyReachedDestination(_damageToBase);
                Recycle();
                return false;
            }
            
            _progress = (_progress - 1f) / _progressFactor;
            PrepareNextState();
            _progress *= _progressFactor;
        }

        if (_directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            _isAlive = false;
        }
    }

    private void PrepareNextState()
    {
        _tileFrom = _tileTo;
        _tileTo = _tileTo.NextTileOnPath;
        _positionFrom = _positionTo;
        if (_tileTo == null)
        {
            PrepareOutro();
        }
        _positionTo = _tileFrom.ExitPoint;
        _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
        _direction = _tileFrom.PathDirection;
        _directionAngleFrom = _directionAngleTo;

        switch (_directionChange)
        {
            case DirectionChange.None:
                PrepareForward();
                break;
            case DirectionChange.TurnRight:
                PrepareTurnRight();
                break;
            case DirectionChange.TurnLeft:
                PrepareTurnLeft();
                break;
            default:
                PrepareAround();
                break;
        }
    }

    private void PrepareForward()
    {
        transform.localRotation = _direction.GetRotation();
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
        _progressFactor = _speed;
    }
    private void PrepareTurnRight()
    {
        _directionAngleTo = _directionAngleFrom + 90f;
        _model.localPosition = new Vector3(_pathOffset - 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
    }
    private void PrepareTurnLeft()
    {
        _directionAngleTo = _directionAngleFrom - 90f;
        _model.localPosition = new Vector3(_pathOffset + 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f + _pathOffset));
    }
    private void PrepareAround()
    {
        _directionAngleTo = _directionAngleFrom + (_pathOffset < 0f ? 180f : -180f);
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localPosition = _positionFrom;
        _progressFactor = _speed / (Mathf.PI * Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
    }

    public override void Recycle()
    {
        OriginFactory.Reclaim(this);
    }

    public void Death()
    {
        Money.AddMoney(_moneyCost);
    }
}
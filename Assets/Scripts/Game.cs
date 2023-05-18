using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private GameBoard _board;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameTileContentFactory _gameTileContentFactory;
    [SerializeField] private EnemyFactory _enemyFactory;
    [SerializeField, Range(0.1f, 10f)] private float _spawnSpeed = 1f;

    private float _spawnProgress;
    private EnemyCollection _enemyCollection = new EnemyCollection();
    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
    
    private void Start()
    {
        _board.Initialize(_boardSize, _gameTileContentFactory);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            HandleTouch();
        else if(Input.GetMouseButtonDown(1))
            HandleAlternativeTouch();

        _spawnProgress += _spawnSpeed * Time.deltaTime;
        if (_spawnProgress >= 1f)
        {
            _spawnProgress -= 1f;
            SpawnEnemy();
        }
        
        _enemyCollection.GameUpdate();
    }

    private void SpawnEnemy()
    {
        GameTile spawnPoint = _board.GetSpawnPoint(UnityEngine.Random.Range(0, _board.SpawnPointCount));
        Enemy enemy = _enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        _enemyCollection.Add(enemy);
    }

    private void HandleTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _board.ToggleTower(tile);
            else
                _board.ToggleWall(tile);
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _board.ToggleSpawnPoint(tile);
            else
                _board.ToggleDestination(tile);
        }
    }
}

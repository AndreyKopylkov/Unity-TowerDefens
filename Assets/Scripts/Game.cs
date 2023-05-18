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
    [SerializeField] private GameScenario _scenario;

    private GameScenario.State _activeScenario;
    private GameBehaviorCollection _enemiesCollection = new GameBehaviorCollection();
    private static Game _instance;
    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private void OnEnable()
    {
        _instance = this;
    }

    private void Start()
    {
        _board.Initialize(_boardSize, _gameTileContentFactory);
        BeginNewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Game will be restart");
            BeginNewGame();
        }
        
        if(Input.GetMouseButtonDown(0))
            HandleTouch();
        else if(Input.GetMouseButtonDown(1))
            HandleAlternativeTouch();

        _activeScenario.Progress();
        _enemiesCollection.GameUpdate();
        Physics.SyncTransforms();
        _board.GameUpdate();
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

    public static void SpawnEnemy(EnemyFactory sequenceFactory, EnemyType sequenceType)
    {
        GameTile spawnPoint = _instance._board.GetSpawnPoint(UnityEngine.Random.Range(0, _instance._board.SpawnPointCount));
        Enemy enemy = sequenceFactory.Get(sequenceType);
        enemy.SpawnOn(spawnPoint);
        _instance._enemiesCollection.Add(enemy);    
    }

    private void BeginNewGame()
    {
        _enemiesCollection.Clear();
        _board.Clear();
        _activeScenario = _scenario.Begin();
    }
}

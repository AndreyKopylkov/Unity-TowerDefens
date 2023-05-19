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
    [SerializeField] private int _startingPlayerHealth = 100;
    [SerializeField] private float _prepareTime = 10f;
    [SerializeField] private int _startingMoney = 100;
    [SerializeField] private Money _money;

    private Coroutine _prepareCoroutine;
    private bool _isScenarioInProcess;
    private int _currentPlayerHealth;
    private GameScenario.State _activeScenario;
    private GameBehaviorCollection _enemiesCollection = new GameBehaviorCollection();
    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
    private bool _isPaused;

    private static Game _instance;
    
    public static event Action<int, int> OnChangeHealth;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPaused = !_isPaused;
            // Time.timeScale = _isPaused ? 0f : 1f;

            if (_isPaused)
            {
                Time.timeScale = 0;
                Debug.Log("Game in pause");
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("Game in non pause anymore");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Game will be restart");
            BeginNewGame();
        }
        
        if(Input.GetMouseButtonDown(0))
            HandleTouch();
        else if(Input.GetMouseButtonDown(1))
            HandleAlternativeTouch();

        if (_isScenarioInProcess)
        {
            if (_currentPlayerHealth <= 0)
            {
                Debug.Log("Defeated!");
                BeginNewGame();
            }

            if (!_activeScenario.Progress() && _enemiesCollection.IsEmpty)
            {
                Debug.Log("Won!");
                Debug.Log("Press the <<R>> button on your keyboard");
            }
            
            _activeScenario.Progress();
            _enemiesCollection.GameUpdate();
        }
        
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
        _isScenarioInProcess = false;
        if (_prepareCoroutine != null)
        {
            StopCoroutine(_prepareCoroutine);
        }
        _enemiesCollection.Clear();
        _board.Clear();
        _currentPlayerHealth = _startingPlayerHealth;
        OnChangeHealth.Invoke(_instance._currentPlayerHealth, _instance._startingPlayerHealth);
        _money.Initialize(_startingMoney);
        
        _prepareCoroutine = StartCoroutine(PrepareRoutine());
    }

    public static void EnemyReachedDestination(int damage)
    {
        _instance._currentPlayerHealth -= damage;
        OnChangeHealth.Invoke(_instance._currentPlayerHealth, _instance._startingPlayerHealth);
    }

    

    private IEnumerator PrepareRoutine()
    {
        Debug.Log($"Wave will start after {_prepareTime} seconds");
        
        yield return new WaitForSeconds(_prepareTime);
        
        _activeScenario = _scenario.Begin();
        _isScenarioInProcess = true;
    }
}

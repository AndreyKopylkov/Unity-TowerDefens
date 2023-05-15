using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private GameBoard _board;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameTileContentFactory _gameTileContentFactory;

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
    
    private void Start()
    {
        _board.Initialize(_boardSize);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            HandleTouch();
    }

    private void HandleTouch()
    {
        GameTile tile = _board.GetTile(TouchRay);
        if (tile != null)
        {
            tile.GameTileContent = _gameTileContentFactory.Get(GameTileContentsType.Destination);
        }
    }
}

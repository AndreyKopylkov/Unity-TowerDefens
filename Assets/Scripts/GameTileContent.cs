using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentsType gameTileContentType = GameTileContentsType.Empty;

    public GameTileContentFactory OriginFactory { get; set; }
    
    public GameTileContentsType GameTileContentType => gameTileContentType;

    public bool IsBlockingPath =>
        GameTileContentType == GameTileContentsType.Wall ||
        GameTileContentType == GameTileContentsType.Tower;
    
    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }

    public virtual void GameUpdate()
    {
        
    }
}

public enum GameTileContentsType { Empty, Destination, Wall, SpawnPoint, Tower}
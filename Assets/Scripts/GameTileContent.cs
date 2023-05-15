using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentsType _gameTileContent = GameTileContentsType.Empty;

    public GameTileContentFactory OriginFactory { get; set; }
    
    public GameTileContentsType GameTileContents => _gameTileContent;
    
    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}

public enum GameTileContentsType { Empty, Destination }
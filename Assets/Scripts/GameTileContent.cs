using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentsType gameTileContentType = GameTileContentsType.Empty;

    public GameTileContentFactory OriginFactory { get; set; }
    
    public GameTileContentsType GameTileContentType => gameTileContentType;
    
    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}

public enum GameTileContentsType { Empty, Destination }
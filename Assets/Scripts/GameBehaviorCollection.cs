using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameBehaviorCollection
{
    private List<GameBehavior> _behaviors = new List<GameBehavior>();
    private List<Transform> _transforms = new List<Transform>();
    
    public void Add(GameBehavior behavior)
    {
        _behaviors.Add(behavior);
        _transforms.Add(behavior.transform);
    }

    public void GameUpdate()
    {
        for (int i = 0; i < _behaviors.Count; i++)
        {
            if (!_behaviors[i].GameUpdate())
            {
                int lastIndex = _behaviors.Count - 1;
                _behaviors[i] = _behaviors[lastIndex];
                _behaviors.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _behaviors.Count; i++)
        {
            _behaviors[i].Recycle();
        }
        _behaviors.Clear();
    }
}
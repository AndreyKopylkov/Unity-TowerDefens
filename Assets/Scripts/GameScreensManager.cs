using System.Collections.Generic;
using UnityEngine;

public class GameScreensManager : MonoBehaviour
{
    [SerializeField] private List<GameScreen> _gameScreens = new List<GameScreen>();

    public void ActiveScreen(ScreenTypes screenType)
    {
        foreach (var screen in _gameScreens)
        {
            screen.gameObject.SetActive(false);
        }
        
        foreach (var screen in _gameScreens)
        {
            if (screen.ScreenType == screenType)
            {
                screen.gameObject.SetActive(true);
            }
        }
    }
}

public enum ScreenTypes {Start, InGame, Defeat, Win}
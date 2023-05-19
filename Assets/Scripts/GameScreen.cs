using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    [SerializeField] private ScreenTypes _screenType = ScreenTypes.InGame;

    public ScreenTypes ScreenType { get { return _screenType; } set {_screenType = value;} }
}
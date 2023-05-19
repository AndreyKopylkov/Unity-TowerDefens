using System;
using UnityEngine;

[CreateAssetMenu]
public class LevelInfo : MonoBehaviour
{
    public static int X = 11;
    public static int Y = 11;

    [System.Serializable]
    public class Column
    {
        public  int[] rows = new int[Y];
    }

    public  Column[] columns = new Column[X];
}
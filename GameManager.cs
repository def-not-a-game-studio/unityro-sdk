using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static long Tick => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

}


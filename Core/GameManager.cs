using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static bool IsOffline = false;

    #region Time

    public static long Tick => _serverTick + (_currentTick - _previousLocalTick);
    public static long Ping = 0;

    private static long _serverTick;
    private static long _previousLocalTick;
    private static long _currentTick => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

    private static long _pingTime = 0;

    public void SetServerTick(long tick) {
        _serverTick = tick * 1000;
        _previousLocalTick = _currentTick;
    }

    #endregion

    private NetworkClient NetworkClient;

    private static void SetPingTime() {
        _pingTime = Tick;
    }

    private static void SetPing() {
        Ping = Tick - _pingTime;
    }

    private void Awake() {
        NetworkClient = FindObjectOfType<NetworkClient>();
    }

    private void Start() {
        NetworkClient.OnPingRequest = SetPingTime;
        
        NetworkClient.HookPacket<ZC.NOTIFY_TIME>(ZC.NOTIFY_TIME.HEADER, OnTimeResponse);
    }

    private void OnTimeResponse(ushort cmd, int size, ZC.NOTIFY_TIME packet) {
        SetPing();
    }
}
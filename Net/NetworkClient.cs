using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ROIO.Utils;
using Telepathy;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Net.Editor;
using UnityRO.Net.Packets;

public class NetworkClient : MonoBehaviour
{
    public const int DATA_BUFFER_SIZE = 2 * 1024;
    public static UnityAction<NetworkPacket, bool> OnPacketEvent;

    #region Singleton

    private static NetworkClient _instance;

    private static NetworkClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkClient>();
            }

            return _instance;
        }
    }

    #endregion

    #region Members

    public bool IsConnected => _client.Connected;

    private static Dictionary<ushort, PacketInfo> RegisteredPackets;
    private static Dictionary<short, int> ClientRegisteredPackets;
    private Dictionary<PacketHeader, List<Delegate>> PacketHooks { get; set; } = new();
    public Action OnPingRequest;

    private bool IsPaused = false;

#if UNITY_EDITOR
    [field: SerializeField] public bool IsRecording { get; private set; }
    public RecordedNetworkTraffic ReplayFile { get; private set; }
    public int ReplayPosition = 0;
    public bool IsReplayStepping;
#endif

    private bool IsInitialized;
    private bool IsReplay;
    private List<RecordedNetworkPacket> ReplayQueue;
    private List<RecordedNetworkPacket> RecordedTraffic = new();

    public NetworkClientState State;

    private Queue<OutPacket> OutPacketQueue;
    private Queue<InPacket> InPacketQueue;

    private Client _client;
    private ServerType _currentServerType = ServerType.Login;
    private Coroutine _pingCoroutine;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        IsInitialized = false;
        DontDestroyOnLoad(this);
        Application.runInBackground = true;
        Log.Error = Debug.LogError;
        Log.Info = Debug.Log;
        Log.Warning = Debug.LogWarning;

        RegisteredPackets = new Dictionary<ushort, PacketInfo>();
        ClientRegisteredPackets = new Dictionary<short, int>();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                     .Where(type => type.GetInterface("InPacket") != null))
        {
            var attributes =
                type.GetCustomAttributes(typeof(PacketHandlerAttribute), true); // get the attributes of the packet.
            if (attributes.Length == 0)
                return;
            var ma = (PacketHandlerAttribute)attributes[0];
            ClientRegisteredPackets.Add((short)ma.MethodId, ma.Size);
            RegisteredPackets.Add(ma.MethodId, new PacketInfo { Size = ma.Size, Type = type });
        }

        SetupClient();
    }

    private void SetupClient()
    {
        _client = new Client(DATA_BUFFER_SIZE);
        // hook up events
        _client.OnConnected = () =>
        {
            if (_currentServerType == ServerType.Zone)
            {
                _pingCoroutine = StartCoroutine(ServerHeartBeat());
            }

            Debug.Log($"# {_currentServerType} Client connected");
        };
        _client.OnData = OnDataReceived;
        _client.OnDisconnected = () => Debug.Log($"# {_currentServerType} Client disconnected");
    }

    public void Start()
    {
        State = new NetworkClientState();

        OutPacketQueue = new Queue<OutPacket>();
        InPacketQueue = new Queue<InPacket>();
    }

    private void Update()
    {
        if (!IsInitialized) return;

#if UNITY_EDITOR
        if (IsReplay)
        {
            HandleReplay();
        }
        else
        {
            HandleLive();
        }
#else
    HandleLive();
#endif
    }

    private void HandleLive()
    {
        // tick to process messages
        // (even if not connected so we still process disconnect messages)
        _client.Tick(100, CheckEnabled);

        if (IsPaused || !_client.Connected)
        {
            if (!_client.Connected)
            {
                Debug.LogError("Disconnected");
            }

            return;
        }

        TrySendPacket();
        TryHandleReceivedPacket();
    }

    private bool CheckEnabled()
    {
        return !IsPaused;
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    #endregion

    public void Connect(string ip, int port, ServerType serverType)
    {
        if (!IsInitialized)
        {
            IsInitialized = true;
        }

        if (IsReplay && IsInitialized)
        {
            return;
        }

        if (_client.Connected)
        {
            _client.Disconnect();
        }

        _currentServerType = serverType;
        _client.Connect(ip, port, ClientRegisteredPackets);

        OutPacketQueue.Clear();
        InPacketQueue.Clear();
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }

    public void HookPacket<T>(PacketHeader cmd, OnPacketReceived<T> onPackedReceived) where T : InPacket
    {
        if (PacketHooks.TryGetValue(cmd, out var delegates))
        {
            delegates.Add(onPackedReceived);
        }
        else
        {
            PacketHooks.Add(cmd, new List<Delegate> { onPackedReceived });
        }
    }

    public void UnhookPacket<T>(PacketHeader cmd, OnPacketReceived<T> onPackedReceived) where T : InPacket
    {
        if (PacketHooks.TryGetValue(cmd, out var delegates) && delegates.Contains(onPackedReceived))
        {
            delegates.Remove(onPackedReceived);
        }
    }


#if UNITY_EDITOR
    private void HandleReplay()
    {
        if (IsReplayStepping) return;
        NextReplayPacket();
    }

    public void NextReplayPacket()
    {
        HandleCurrentReplayPacket();
        ReplayPosition++;
    }

    public void PreviousReplayPacket()
    {
        if (ReplayPosition <= 0) return;
        ReplayPosition--;
        HandleCurrentReplayPacket();
    }

    private void HandleCurrentReplayPacket()
    {
        if (ReplayPosition < ReplayQueue.Count - 1)
        {
            var packet = ReplayQueue[ReplayPosition];
            if (packet.IsOut)
            {
                // do nothing, cant really send it anywhere
            }
            else
            {
                OnDataReceived(new ArraySegment<byte>(packet.Data));
            }
        }
    }

    public void StartReplay(RecordedNetworkTraffic replayFile, bool isReplayStepping = false)
    {
        ReplayFile = replayFile;
        IsReplayStepping = isReplayStepping;
        IsReplay = true;
        ReplayPosition = 0;
        ReplayQueue = new List<RecordedNetworkPacket>(ReplayFile.Packets);
    }

    public void StartRecording()
    {
        IsRecording = true;
        RecordedTraffic = new List<RecordedNetworkPacket>();
    }

    public void StopRecording(string fileName)
    {
        IsRecording = false;
        var recorded = ScriptableObject.CreateInstance<RecordedNetworkTraffic>();

        recorded.Packets = RecordedTraffic.ToArray();
        RecordedTraffic.Clear();

        if (!Directory.Exists("Assets/Misc/Replays/"))
        {
            Directory.CreateDirectory("Assets/Misc/Replays/");
        }

        UnityEditor.AssetDatabase.CreateAsset(recorded,
            UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/Misc/Replays/{fileName}.asset"));
    }
#endif

    #region Packet Handling

    private MemoryStreamReader MemoryStream;
    private byte[] commandBuffer = new byte[2];
    private byte[] sizeBuffer = new byte[2];

    private void OnDataReceived(ArraySegment<byte> byteArray)
    {
        if (byteArray.Array == null) return;

#if UNITY_EDITOR
        if (IsRecording)
        {
            using var ms = new MemoryStream(byteArray.Array);
            var recordedPacket = new RecordedNetworkPacket
                { Time = Time.realtimeSinceStartup, Data = ms.ToArray(), IsOut = false };
            RecordedTraffic.Add(recordedPacket);
        }
#endif

        MemoryStream = new MemoryStreamReader(byteArray.Array);
        MemoryStream.Read(commandBuffer, 0, 2);

        var cmd = BitConverter.ToUInt16(commandBuffer, 0);

        Debug.Log($"Received packet {cmd} ({cmd:x4})");
        if (RegisteredPackets.ContainsKey(cmd))
        {
            var size = RegisteredPackets[cmd].Size;
            var isFixed = true;

            if (size <= 0)
            {
                isFixed = false;
                if (MemoryStream.Length - MemoryStream.Position >= 2)
                {
                    MemoryStream.Read(sizeBuffer, 0, 2);
                    size = BitConverter.ToUInt16(sizeBuffer, 0);
                }
            }

            var ci = RegisteredPackets[cmd].Type.GetConstructor(new Type[] { });
            var packet = (InPacket)ci.Invoke(null);
            packet.Read(MemoryStream, size - (isFixed ? 2 : 4));

            if (IsReplay)
            {
                HandleIncomingPacket(packet);
            }
            else
            {
                OnDataReceived(packet);
            }
        }
        else if (ClientRegisteredPackets.ContainsKey((short)cmd))
        {
            // this is a hack
            // the reason why we need this is because the server sends
            // our account id back as a packet, so if we don't register
            // the first two bytes of the account id's int, we'll get disconnected

            // do nothing
        }
        else
        {
            Debug.LogError($"Received invalid packet {cmd} ({cmd:x4})");
        }
    }

    public void PausePacketHandling()
    {
        IsPaused = true;
    }

    public void ResumePacketHandling()
    {
        IsPaused = false;
    }

    public void OnDataReceived(InPacket packet)
    {
        if (IsPaused)
        {
            InPacketQueue.Enqueue(packet);
        }
        else
        {
            HandleIncomingPacket(packet);
        }
    }

    public static void SendPacket(OutPacket packet)
    {
        if (Instance?.IsPaused == true || Instance?.IsConnected == false)
        {
            Instance?.OutPacketQueue.Enqueue(packet);
        }
        else
        {
            Instance?.HandleOutPacket(packet);
        }
    }

    private void TrySendPacket()
    {
        if (OutPacketQueue.Count == 0 || !IsConnected)
        {
            return;
        }

        while (OutPacketQueue.TryDequeue(out var packet))
        {
            HandleOutPacket(packet);
        }
    }

    private void HandleOutPacket(OutPacket packet)
    {
        var byteArray = packet.AsArraySegment();
#if UNITY_EDITOR
        if (IsRecording)
        {
            RecordedTraffic.Add(new RecordedNetworkPacket
                { Time = Time.realtimeSinceStartup, Data = byteArray.Array, IsOut = true });
        }
#endif

        _client.Send(byteArray);
        OnPacketEvent?.Invoke(packet, true);
    }

    private void TryHandleReceivedPacket()
    {
        while (InPacketQueue.TryDequeue(out var packet))
        {
            HandleIncomingPacket(packet);
        }
    }

    private bool HandleIncomingPacket(InPacket packet)
    {
        var isHandled = PacketHooks.TryGetValue(packet.Header, out var delegates);

        if (delegates != null)
        {
            foreach (var d in delegates)
            {
                d.DynamicInvoke((ushort)packet.Header, -1, packet);
            }
        }

        OnPacketEvent?.Invoke(packet, isHandled);

        return isHandled;
    }

    // non orthodox alternative fix
    // when we connect to rA charserv, they send the accountId back
    // out of nowwhere, no header, just the plain 4bytes
    public void SetAccountId(int AID)
    {
        var bytes = BitConverter.GetBytes(AID);
        ClientRegisteredPackets.Add(BitConverter.ToInt16(bytes, 0), 4);
    }

    #endregion

    private IEnumerator ServerHeartBeat()
    {
        for (;;)
        {
            if (!IsConnected) yield break;
            OnPingRequest?.Invoke();
            new CZ.REQUEST_TIME2().Send();
            yield return new WaitForSeconds(1f);
        }
    }

    public struct NetworkClientState
    {
        public MapLoginInfo MapLoginInfo;
        public CharServerInfo CharServer;
        public CharacterData SelectedCharacter;
        public AC.ACCEPT_LOGIN3 LoginInfo;
        public HC.ACCEPT_ENTER CurrentCharactersInfo;
    }

    public enum ServerType
    {
        Login,
        Char,
        Zone
    }

    public delegate void OnPacketReceived<T>(ushort cmd, int size, T packet) where T : InPacket;
}
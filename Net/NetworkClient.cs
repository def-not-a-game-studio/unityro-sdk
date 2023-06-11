using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ROIO.Utils;
using Telepathy;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Net.Packets;

public class NetworkClient : MonoBehaviour {
    public const int DATA_BUFFER_SIZE = 2 * 1024;
    public static UnityAction<NetworkPacket, bool> OnPacketEvent;

    #region Singleton

    private static NetworkClient _instance;
    private static NetworkClient Instance {
        get {
            if (_instance == null) {
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

    public NetworkClientState State;

    private Queue<OutPacket> OutPacketQueue;
    private Queue<InPacket> InPacketQueue;

    private Telepathy.Client _client;
    private ServerType _currentServerType = ServerType.Login;
    private Coroutine _pingCoroutine;

    #endregion

    #region Lifecycle

    private void Awake() {
        DontDestroyOnLoad(this);
        Application.runInBackground = true;
        Telepathy.Log.Error = Debug.LogError;
        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        
        RegisteredPackets = new Dictionary<ushort, PacketInfo>();
        ClientRegisteredPackets = new Dictionary<short, int>();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetInterface("InPacket") != null)) {
            var attributes = type.GetCustomAttributes(typeof(PacketHandlerAttribute), true); // get the attributes of the packet.
            if (attributes.Length == 0)
                return;
            var ma = (PacketHandlerAttribute) attributes[0];
            ClientRegisteredPackets.Add((short)ma.MethodId, ma.Size);
            RegisteredPackets.Add(ma.MethodId, new PacketInfo { Size = ma.Size, Type = type });
        }

        SetupClient();
    }

    private void SetupClient() {
        _client = new Client(DATA_BUFFER_SIZE);
        // hook up events
        _client.OnConnected = () => {
            if (_currentServerType == ServerType.Zone) {
                _pingCoroutine = StartCoroutine(ServerHeartBeat());
            }
            Debug.Log($"# {_currentServerType} Client connected");
        };
        _client.OnData = OnDataReceived;
        _client.OnDisconnected = () => Debug.Log($"# {_currentServerType} Client disconnected");
    }

    public void Start() {
        State = new NetworkClientState();

        OutPacketQueue = new Queue<OutPacket>();
        InPacketQueue = new Queue<InPacket>();
    }

    private void Update() {
        // tick to process messages
        // (even if not connected so we still process disconnect messages)
        _client.Tick(100, CheckEnabled);

        if (IsPaused || !_client.Connected) {
            return;
        }

        TrySendPacket();
        TryHandleReceivedPacket();
    }

    private bool CheckEnabled() {
        return !IsPaused;
    }

    private void OnApplicationQuit() {
        Disconnect();
    }

    #endregion

    public void Connect(string ip, int port, ServerType serverType) {
        if (_client.Connected) {
            _client.Disconnect();
        }
        
        _currentServerType = serverType;
        _client.Connect(ip, port, ClientRegisteredPackets);

        OutPacketQueue.Clear();
        InPacketQueue.Clear();
    }

    public void Disconnect() {
        _client.Disconnect();
    }

    public void HookPacket<T>(PacketHeader cmd, OnPacketReceived<T> onPackedReceived) where T : InPacket {
        if (PacketHooks.TryGetValue(cmd, out var delegates)) {
            delegates.Add(onPackedReceived);
        } else {
            PacketHooks.Add(cmd, new List<Delegate> { onPackedReceived });
        }
    }

    public void UnhookPacket<T>(PacketHeader cmd, OnPacketReceived<T> onPackedReceived) where T : InPacket {
        if (PacketHooks.TryGetValue(cmd, out var delegates) && delegates.Contains(onPackedReceived)) {
            delegates.Remove(onPackedReceived);
        }
    }

    #region Packet Handling

    private MemoryStreamReader MemoryStream;
    private byte[] commandBuffer = new byte[2];
    private byte[] sizeBuffer = new byte[2];

    private void OnDataReceived(ArraySegment<byte> byteArray) {
        if (byteArray.Array == null) return;

        MemoryStream = new MemoryStreamReader(byteArray.Array);

        MemoryStream.Read(commandBuffer, 0, 2);
        var cmd = BitConverter.ToUInt16(commandBuffer, 0);
        if (RegisteredPackets.ContainsKey(cmd)) {
            var size = RegisteredPackets[cmd].Size;
            var isFixed = true;

            if (size <= 0) {
                isFixed = false;
                if (MemoryStream.Length - MemoryStream.Position >= 2) {
                    MemoryStream.Read(sizeBuffer, 0, 2);
                    size = BitConverter.ToUInt16(sizeBuffer, 0);
                }
            }

            var ci = RegisteredPackets[cmd].Type.GetConstructor(new Type[] { });
            var packet = (InPacket)ci.Invoke(null);
            packet.Read(MemoryStream, size - (isFixed ? 2 : 4));

            OnDataReceived(packet);
        } else if (ClientRegisteredPackets.ContainsKey((short)cmd)) {
            // this is a hack
            // the reason why we need this is because the server sends
            // our account id back as a packet, so if we don't register
            // the first two bytes of the account id's int, we'll get disconnected
            
            // do nothing
        } else {
            Debug.LogError($"Received invalid packet {cmd} ({cmd:x4})");
        }
    }

    public void PausePacketHandling() {
        IsPaused = true;
    }

    public void ResumePacketHandling() {
        IsPaused = false;
    }

    public void OnDataReceived(InPacket packet) {
        if (IsPaused) {
            InPacketQueue.Enqueue(packet);
        } else {
            HandleIncomingPacket(packet);
        }
    }

    public static void SendPacket(OutPacket packet) {
        if (Instance?.IsPaused == true || Instance?.IsConnected == false) {
            Instance?.OutPacketQueue.Enqueue(packet);
        } else {
            Instance?.HandleOutPacket(packet);
        }
    }

    private void TrySendPacket() {
        if (OutPacketQueue.Count == 0 || !IsConnected) {
            return;
        }

        while (OutPacketQueue.TryDequeue(out var packet)) {
            HandleOutPacket(packet);
        }
    }

    private void HandleOutPacket(OutPacket packet) {
        _client.Send(packet.AsArraySegment());
        OnPacketEvent?.Invoke(packet, true);
    }

    private void TryHandleReceivedPacket() {
        while (InPacketQueue.TryDequeue(out var packet)) {
            HandleIncomingPacket(packet);
        }
    }

    private bool HandleIncomingPacket(InPacket packet) {
        var isHandled = PacketHooks.TryGetValue(packet.Header, out var delegates);

        if (delegates != null) {
            foreach (var d in delegates) {
                d.DynamicInvoke((ushort)packet.Header, -1, packet);
            }
        }

        OnPacketEvent?.Invoke(packet, isHandled);

        return isHandled;
    }

    // non orthodox alternative fix
    // when we connect to rA charserv, they send the accountId back
    // out of nowwhere, no header, just the plain 4bytes
    public void SetAccountId(int AID) {
        var bytes = BitConverter.GetBytes(AID);
        ClientRegisteredPackets.Add(BitConverter.ToInt16(bytes, 0), 4);
    }

    #endregion

    private IEnumerator ServerHeartBeat() {
        for (;;) {
            if (!IsConnected) yield break;
            OnPingRequest?.Invoke();
            new CZ.REQUEST_TIME2().Send();
            yield return new WaitForSeconds(1f);
        }
    }

    public struct NetworkClientState {
        public MapLoginInfo MapLoginInfo;
        public CharServerInfo CharServer;
        public CharacterData SelectedCharacter;
        public AC.ACCEPT_LOGIN3 LoginInfo;
        public HC.ACCEPT_ENTER CurrentCharactersInfo;
    }

    public enum ServerType {
        Login,
        Char,
        Zone
    }

    public delegate void OnPacketReceived<T>(ushort cmd, int size, T packet) where T : InPacket;
}
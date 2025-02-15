using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ROIO.Utils;
using Telepathy;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Net.Packets;

namespace UnityRO.Net
{
    public partial class NetworkClient : MonoBehaviour
    {
        public const int DATA_BUFFER_SIZE = 2 * 1024;
        public static UnityAction<NetworkPacket, bool> OnPacketEvent;

        #region Singleton

        private static NetworkClient _instance;

        private static NetworkClient Instance => _instance ??= FindFirstObjectByType<NetworkClient>();

        #endregion

        #region Members

        public bool IsConnected => _client.Connected;

        private static Dictionary<ushort, PacketInfo> RegisteredPackets;
        private static Dictionary<short, int> ClientRegisteredPackets;
        private Dictionary<PacketHeader, List<Delegate>> PacketHooks { get; set; } = new();
        public Action OnPingRequest;

        private bool IsPaused = false;
        private bool IsInitialized;
        private bool IsReplay;

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

            FillRegisteredPackets();
            SetupClient();
        }

        private void FillRegisteredPackets()
        {
            var packets = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetInterface("InPacket") != null);
            foreach (var type in packets)
            {
                var attributes =
                    type.GetCustomAttributes(typeof(PacketHandlerAttribute), true); // get the attributes of the packet.
                
                if (attributes.Length == 0)
                    continue;
                
                var ma = (PacketHandlerAttribute)attributes[0];
                ClientRegisteredPackets.Add((short)ma.MethodId, ma.Size);
                RegisteredPackets.Add(ma.MethodId, new PacketInfo { Size = ma.Size, Type = type });
            }
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

#if UNITY_EDITOR
        private void Update()
        {
            if (!IsInitialized) return;

            if (IsReplay)
                HandleReplay();
            else
                HandleLive();
        }
#else
        private void Update()
        {
            if (!IsInitialized) return;
            HandleLive();
        }
#endif

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
#if UNITY_EDITOR
                    if (_currentServerType == ServerType.Zone && Application.isPlaying)
                    {
                        EditorApplication.ExitPlaymode();
                    }
#endif
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

            // TODO: check if serverType is different than current serverType and allow
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
}
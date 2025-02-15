using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityRO.Net
{
    public partial class NetworkClient
    {
        
        [field: SerializeField] public bool IsRecording { get; private set; }
        public RecordedNetworkTraffic ReplayFile { get; private set; }
        public int ReplayPosition = 0;
        public bool IsReplayStepping;
        private List<RecordedNetworkPacket> ReplayQueue;
        private List<RecordedNetworkPacket> RecordedTraffic = new();
        
        
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
    }
}
#endif
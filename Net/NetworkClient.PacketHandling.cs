using System;
using System.IO;
using ROIO.Utils;
using UnityEngine;

namespace UnityRO.Net
{
    public partial class NetworkClient
    {
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
                {
                    Time = Time.realtimeSinceStartup,
                    Data = ms.ToArray(),
                    IsOut = false
                };
                RecordedTraffic.Add(recordedPacket);
            }
#endif

            MemoryStream = new MemoryStreamReader(byteArray.Array);
            MemoryStream.Read(commandBuffer, 0, 2);

            var cmd = BitConverter.ToUInt16(commandBuffer, 0);

            //Debug.Log($"Received packet {cmd} ({cmd:x4})");
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
                // the reason why we need this is that the server sends
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

        private void HandleIncomingPacket(InPacket packet)
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
        }

        // non-orthodox alternative fix
        // when we connect to rA charserv, they send the accountId back
        // out of nowwhere, no header, just the plain 4bytes
        public void SetAccountId(int AID)
        {
            var bytes = BitConverter.GetBytes(AID);
            ClientRegisteredPackets.Add(BitConverter.ToInt16(bytes, 0), 4);
        }
    }
}
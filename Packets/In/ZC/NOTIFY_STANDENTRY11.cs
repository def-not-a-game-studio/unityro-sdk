﻿using ROIO.Utils;

public partial class ZC {

    [PacketHandler(HEADER, "ZC_NOTIFY_STANDENTRY11")]
    public class NOTIFY_STANDENTRY11 : InPacket {

        public const PacketHeader HEADER = PacketHeader.ZC_NOTIFY_STANDENTRY11;
        public PacketHeader Header => HEADER;

        public EntitySpawnData entityData;

        public void Read(MemoryStreamReader br, int size) {
            entityData = new EntitySpawnData();

            entityData.objecttype = br.ReadByte();
            entityData.AID = br.ReadUInt();
            entityData.GID = br.ReadUInt();
            entityData.speed = br.ReadShort();
            entityData.bodyState = br.ReadShort();
            entityData.healthState = br.ReadShort();
            entityData.effectState = br.ReadInt();
            entityData.job = br.ReadShort();
            entityData.head = br.ReadUShort();
            entityData.Weapon = br.ReadUInt();
            entityData.Shield = br.ReadUInt();
            entityData.Accessory = br.ReadUShort();
            entityData.Accessory2 = br.ReadUShort();
            entityData.Accessory3 = br.ReadUShort();
            entityData.HairColor = br.ReadShort();
            entityData.ClothesColor = br.ReadShort();
            entityData.headDir = br.ReadShort();
            entityData.Robe = br.ReadUShort();
            entityData.GuildID = br.ReadUInt();
            entityData.GEmblemVer = br.ReadShort();
            entityData.honor = br.ReadShort();
            entityData.virtue = br.ReadInt();
            entityData.isPKModeON = (byte)br.ReadByte();
            entityData.sex = (byte)br.ReadByte();
            entityData.PosDir = br.ReadPos();
            entityData.xSize = (byte)br.ReadByte();
            entityData.ySize = (byte)br.ReadByte();
            entityData.state = EntitySpawnData.EntitySpawnState.Stand;
            entityData.clevel = br.ReadShort();
            entityData.font = br.ReadShort();
            entityData.MaxHP = br.ReadInt();
            entityData.HP = br.ReadInt();
            entityData.isBoss = (byte)br.ReadByte();
            entityData.body = br.ReadUShort();
            entityData.name = br.ReadBinaryString(24);
        }
    }
}

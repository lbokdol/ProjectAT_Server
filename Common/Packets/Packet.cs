using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;

namespace Common.Packet
{
    [MessagePackObject]
    public class Packet
    {
        [Key(0)]
        public PacketType Type { get; set; }

        [Key(1)]
        public Guid UserId { get; set; }

        [Key(2)]
        public string Signature { get; set; }

        [Key(3)]
        public byte[] Data { get; set; }

        public byte[] GetSignatureData()
        {
            return Encoding.UTF8.GetBytes(Type.ToString() + UserId.ToString() + BitConverter.ToString(Data));
        }
    }

    public enum PacketType
    {
        LoginRequest,
        LoginResponse,
        MoveRequest,
        MoveResponse,
        // Add more packet types as needed
    }

    [MessagePackObject]
    public class LoginRequest
    {
        [Key(0)]
        public string Username { get; set; }
        [Key(1)]
        public string Password { get; set; }
    }

    [MessagePackObject]
    public class LoginResponse
    {
        [Key(0)]
        public bool Result { get; set; }
    }

    [MessagePackObject]
    public class MoveResponse
    {
        [Key(0)]
        public bool Result { get; set; }
    }

    [MessagePackObject]
    public class MoveData
    {
        [Key(0)]
        public Guid PlayerId { get; set; }
        [Key(1)]
        public double NewPosition { get; set; }
    }

    [MessagePackObject]
    public class PlayerData
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public Guid PlayerId { get; set; }
        [Key(2)]
        public int Level { get; set; }
        [Key(3)]
        public int Experience { get; set; }
        [Key(4)]
        public int Health { get; set; }
        [Key(5)]
        public int Mana { get; set; }
        [Key(6)]
        public int Strength { get; set; }
        [Key(7)]
        public int Dexterity { get; set; }
        [Key(8)]
        public int Intelligence { get; set; }
    }
}

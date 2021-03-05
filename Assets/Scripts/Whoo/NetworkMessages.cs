namespace Whoo
{
    public enum MessageType : byte
    {
        Empty          = 0x00,
        RefreshRequest = 0x01
    }

    public static class NetworkMessages
    {
        public static bool HasNetworkCapability(this WhooRoom room)
        {
            return room?.RoomGroup != null && room.RoomGroup.IsAlive;
        }

        public static void EmptyMessage(this WhooRoom room)
        {
            if (!room.HasNetworkCapability()) return;
            room.RoomGroup.Broadcast(new[] {(byte) MessageType.Empty});
        }

        public static void RequestRefreshOnOccupants(this WhooRoom room)
        {
            if (!room.HasNetworkCapability()) return;
            room.RoomGroup.Broadcast(new[] {(byte) MessageType.RefreshRequest});
        }
    }
}
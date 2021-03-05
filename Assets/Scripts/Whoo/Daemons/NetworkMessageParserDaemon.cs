using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whoo.Daemons
{
    public class NetworkMessageParserDaemon : DaemonBase
    {
        protected INetworkGroup groupOverride;
        protected INetworkGroup Group => groupOverride ?? Room.RoomGroup;

        public NetworkMessageParserDaemon(WhooRoom room, INetworkGroup groupOverride = null) : base(room)
        {
            this.groupOverride = groupOverride;
        }

        public override async UniTask<bool> CanAttach()
        {
            return await UniTask.FromResult(Room.HasNetworkCapability());
        }

        public override void Run()
        {
            Group.OnBroadcastReceived += BroadcastHandler;
        }

        public override void Dispose()
        {
            base.Dispose();
            Group.OnBroadcastReceived -= BroadcastHandler;
        }

        private void BroadcastHandler(long sender, byte[] data)
        {
            if (sender.ToString() == Group.LocalUser.UniqueId) return; //ignore loopback for now

            if (data.Length == 0) return; //ignore empty messages

            switch (data[0])
            {
                case (byte) MessageType.Empty:
                    Debug.Log($"Empty message broadcast {sender}.");
                    break;
                case (byte) MessageType.RefreshRequest:
                    Room.StrapiRoom.Refresh().Forget();
                    break;
                default:
                    Debug.Log("Unknown message received over the network.");
                    break;
            }
        }
    }
}
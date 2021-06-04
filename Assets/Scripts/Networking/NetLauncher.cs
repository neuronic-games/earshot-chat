using MLAPI;

namespace Networking
{
    public class NetLauncher : NetworkBehaviour
    {
        public static void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            //SetPlayerInactive();
        }

        public static void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            //SetPlayerInactive();
        }

        private void SetPlayerInactive()
        {
            if (IsOwner)
            {
                // find a player and deactivate it
            }
        }
    }
}
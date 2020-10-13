using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using UnityEngine;

namespace AppLayer
{
    public class LogAppLayer : IAppLayer
    {
        public readonly IAppLayer Logged;

        private long _LogMsgId = 0;

        private string LogMsgPrefix => $"[Log:{_LogMsgId++}]";

        public LogAppLayer(IAppLayer logged, bool attachLoggersToListeners)
        {
            Logged = logged;
            if (attachLoggersToListeners)
            {
                AttachLoggersToListeners();
            }
        }

        private IGroupCallbacks   _groupLogger   = new LoggingNetworkGroup();
        private IOverlayCallbacks _overlayLogger = new LoggingOverlay();
        private IUserCallbacks    _userLogger    = new LoggingUser();
        private IVoiceCallbacks   _voiceLogger   = new LoggingVoice();

        private void AttachLoggersToListeners()
        {
            Logged.RegisterCallbacks(_groupLogger);
            Logged.RegisterCallbacks(_overlayLogger);
            Logged.RegisterCallbacks(_userLogger);
            Logged.RegisterCallbacks(_voiceLogger);
        }

        private void RemoveLoggersFromListeners()
        {
            Logged.RemoveCallbacks(_groupLogger);
            Logged.RemoveCallbacks(_overlayLogger);
            Logged.RemoveCallbacks(_userLogger);
            Logged.RemoveCallbacks(_voiceLogger);
        }

        #region Logging Listeners Callbacks

        public class LoggingNetworkGroup : IGroupCallbacks
        {
            public void OnLobbyUpdate(long lobbyid)
            {
                Debug.Log($"Lobby {lobbyid} updated.");
            }

            public void OnNetworkMessage(long lobbyid, long userid, byte channelid, byte[] data)
            {
                Debug.Log(
                    $"Network messaged received in lobby {lobbyid} from member {userid} in channel {channelid}.\n Length is {data.Length} and the data is:-\n{data}");
            }

            public void OnMemberConnect(long lobbyid, long userid)
            {
                Debug.Log($"Member {userid} connected to lobby {lobbyid}.");
            }

            public void OnSpeaking(long lobbyid, long userid, bool speaking)
            {
                if (speaking)
                {
                    Debug.Log($"Member {userid} speaking in lobby {lobbyid}.");
                }
                else
                {
                    Debug.Log($"Member {userid} went silent in lobby {lobbyid}.");
                }
            }

            public void OnMemberUpdate(long lobbyid, long userid)
            {
                Debug.Log($"Member {userid} in lobby {lobbyid} has updated their metadata.");
            }

            public void OnMemberDisconnect(long lobbyid, long userid)
            {
                Debug.Log($"Member {userid} left lobby {lobbyid}.");
            }

            public void OnLobbyDelete(long lobbyid, uint reason)
            {
                Debug.Log($"Lobby {lobbyid} deleted.");
            }

            public void OnLobbyMessage(long lobbyid, long userid, byte[] data)
            {
                Debug.Log(
                    $"Message received in lobby {lobbyid} from member {userid} of length {data.Length}. Data:-\n{data}");
            }
        }

        public class LoggingVoice : IVoiceCallbacks
        {
            public void OnSettingsUpdate()
            {
                Debug.Log($"Voice settings updated.");
            }
        }

        public class LoggingOverlay : IOverlayCallbacks
        {
            public void OnToggle(bool manager)
            {
                Debug.Log($"Overlay is open: {manager}.");
            }
        }

        public class LoggingUser : IUserCallbacks
        {
            public void OnCurrentUserUpdate(IUser localUser)
            {
                Debug.Log($"Current user updated. {localUser}");
            }
        }

        #endregion

        #region IAppLayer

        public void DestroyApp()
        {
            Debug.Log($"{GetType().Name}.{nameof(DestroyApp)}");
            RemoveLoggersFromListeners();
            Logged.DestroyApp();
        }

        public void RegisterCallbacks(IGroupCallbacks listener)
        {
            Debug.Log($"Registered {nameof(IGroupCallbacks)} : {listener}.");
            Logged.RegisterCallbacks(listener);
        }

        public void RegisterCallbacks(IVoiceCallbacks listener)
        {
            Debug.Log($"Registered callbacks of {nameof(IVoiceCallbacks)} to {listener}.");
            Logged.RegisterCallbacks(listener);
        }

        public void RegisterCallbacks(IOverlayCallbacks listener)
        {
            Debug.Log($"Registered callbacks of {nameof(IOverlayCallbacks)} to {listener}.");
            Logged.RegisterCallbacks(listener);
        }

        public void RegisterCallbacks(IUserCallbacks listener)
        {
            Debug.Log($"Registered callbacks of {nameof(IUser)} to {listener}.");
            Logged.RegisterCallbacks(listener);
        }

        public void RemoveCallbacks(IGroupCallbacks listener)
        {
            Debug.Log($"Removed callbacks of {nameof(IGroupCallbacks)} to {listener}.");
            Logged.RemoveCallbacks(listener);
        }

        public void RemoveCallbacks(IVoiceCallbacks listener)
        {
            Debug.Log($"Removed callbacks of {nameof(IVoiceCallbacks)} to {listener}.");
            Logged.RemoveCallbacks(listener);
        }

        public void RemoveCallbacks(IOverlayCallbacks listener)
        {
            Debug.Log($"Removed callbacks of {nameof(IOverlayCallbacks)} to {listener}.");
            Logged.RemoveCallbacks(listener);
        }

        public void RemoveCallbacks(IUserCallbacks listener)
        {
            Debug.Log($"Removed callbacks of {nameof(IUserCallbacks)} to {listener}.");
            Logged.RemoveCallbacks(listener);
        }

        public IUser                        LocalUser   => Logged.LocalUser;
        public IReadOnlyList<IUser>         KnownUsers  => Logged.KnownUsers;
        public IReadOnlyList<INetworkGroup> KnownGroups => Logged.KnownGroups;

        #endregion
    }
}
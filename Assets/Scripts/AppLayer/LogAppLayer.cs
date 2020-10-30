using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using Discord;
using DiscordAppLayer;
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
            if (logged is DiscordApp app)
            {
                _groupLogger = new LoggingDiscordGroup(app);
            }
            else
            {
                _groupLogger = new LoggingNetworkGroup();
            }

            if (attachLoggersToListeners)
            {
                AttachLoggersToListeners();
            }
        }

        private IGroupCallbacks    _groupLogger    = null;
        private IOverlayCallbacks  _overlayLogger  = new LoggingOverlay();
        private IUserCallbacks     _userLogger     = new LoggingUser();
        private IVoiceCallbacks    _voiceLogger    = new LoggingVoice();
        private IActivityCallbacks _activityLogger = new LoggingActivity();

        private void AttachLoggersToListeners()
        {
            Logged.RegisterCallbacks(_groupLogger);
            Logged.RegisterCallbacks(_overlayLogger);
            Logged.RegisterCallbacks(_userLogger);
            Logged.RegisterCallbacks(_voiceLogger);

            if (Logged is DiscordApp discord)
            {
                discord.RegisterCallbacks(_activityLogger);
            }
        }

        private void RemoveLoggersFromListeners()
        {
            Logged.RemoveCallbacks(_groupLogger);
            Logged.RemoveCallbacks(_overlayLogger);
            Logged.RemoveCallbacks(_userLogger);
            Logged.RemoveCallbacks(_voiceLogger);

            if (Logged is DiscordApp discord)
            {
                discord.RemoveCallbacks(_activityLogger);
            }
        }

        #region Logging Listeners Callbacks

        public class LoggingNetworkGroup : IGroupCallbacks
        {
            public virtual void OnLobbyUpdate(long lobbyid)
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

            public virtual void OnMemberUpdate(long lobbyid, long userid)
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

        public sealed class LoggingDiscordGroup : LoggingNetworkGroup
        {
            public readonly DiscordApp App;

            public LoggingDiscordGroup(DiscordApp app)
            {
                App = app;
            }

            private StringBuilder _sb = new StringBuilder();

            public override void OnLobbyUpdate(long lobbyid)
            {
                _sb.Clear();

                var manager   = App.LobbyManager;
                int metaCount = manager.LobbyMetadataCount(lobbyid);
                _sb.AppendLine($"Lobby {lobbyid} has updated. New Metadata Count: {metaCount}.");
                for (int i = 0; i < metaCount; i++)
                {
                    string key   = manager.GetLobbyMetadataKey(lobbyid, i);
                    string value = manager.GetLobbyMetadataValue(lobbyid, key);
                    _sb.AppendLine($"\t{key} : {value}");
                }

                Debug.Log(_sb.ToString());
            }

            public override void OnMemberUpdate(long lobbyid, long userid)
            {
                _sb.Clear();

                var manager   = App.LobbyManager;
                int metaCount = manager.MemberMetadataCount(lobbyid, userid);
                _sb.AppendLine($"User {userid} in {lobbyid} has updated. New Metadata Count: {metaCount}.");
                for (int i = 0; i < metaCount; i++)
                {
                    string key   = manager.GetMemberMetadataKey(lobbyid, userid, i);
                    string value = manager.GetMemberMetadataValue(lobbyid, userid, key);
                    _sb.AppendLine($"\t{key} : {value}");
                }

                Debug.Log(_sb.ToString());
            }
        }

        public sealed class LoggingVoice : IVoiceCallbacks
        {
            public void OnSettingsUpdate()
            {
                Debug.Log($"Voice settings updated.");
            }
        }

        public sealed class LoggingOverlay : IOverlayCallbacks
        {
            public void OnToggle(bool manager)
            {
                if (DiscordApp.GetDiscordApp(out var app))
                {
                    var o = app.OverlayManager;
                    Debug.Log($"Overlay toggled.\nIsEnabled: {o.IsEnabled()}\nIsLocked: {o.IsLocked()}");
                }
                else
                {
                    Debug.Log($"Overlay is open: {manager}.");
                }
            }
        }

        public sealed class LoggingUser : IUserCallbacks
        {
            public void OnCurrentUserUpdate()
            {
                Debug.Log($"Current user updated.\n");
            } /*public void OnCurrentUserUpdate(IUser localUser)
            {
                Debug.Log($"Current user updated.\n{localUser}");
            }*/
        }

        public sealed class LoggingActivity : IActivityCallbacks
        {
            public void OnActivityJoin(ActivityManager activityManager, string secret)
            {
                Debug.Log($"Joined through activity. Secret: {secret}.");
            }

            public void OnActivityJoinRequest(ActivityManager activityManager, ref User user)
            {
                Debug.Log($"Requested to join by user {user.Id}/{user.Username}.");
            }

            public void OnActivitySpectate(ActivityManager activityManager, string secret)
            {
                Debug.Log($"Spectating through activity. Secret: {secret}.");
            }

            public void OnActivityInvite(ActivityManager activityManager, ActivityActionType type, ref User user,
                ref Activity                             activity)
            {
                Debug.Log(
                    $"Invite through Activity.\nType: {type}\nUser: {user.Id}/{user.Username}\nActivity: {activity}");
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

        #region Registration

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

        #endregion

        #region Relationships

        public IReadOnlyList<IUser>         KnownUsers  => Logged.KnownUsers;
        public IReadOnlyList<INetworkGroup> KnownGroups => Logged.KnownGroups;

        #endregion

        #region Factory

        public int  GroupCapacity  => Logged.GroupCapacity;
        public bool CanCreateGroup => Logged.CanCreateGroup;

        public bool CanJoinGroup => Logged.CanJoinGroup;

        public void CreateNewGroup(uint capacity, bool locked, Action<INetworkGroup> onCreated)
        {
            Debug.Log($"Requesting creation of new group.");
            Logged.CreateNewGroup(capacity, locked, group =>
            {
                if (group == null) Debug.Log($"Failed to create group.");
                else Debug.Log($"Created group.\n{group}");
                onCreated?.Invoke(group);
            });
        }

        public void JoinGroup(long groupId, string secret, Action<INetworkGroup> onJoined)
        {
            Debug.Log($"Requesting joining of group with id {groupId} and secret {secret}.");
            Logged.JoinGroup(groupId, secret, group =>
            {
                onJoined?.Invoke(group); //invoke before so everything is ready for printing.
                if (group == null) Debug.Log($"Failed to join group.");
                else Debug.Log($"Joined group.\n{group}");
            });
        }

        public void DeleteGroup(INetworkGroup @group)
        {
            Debug.Log($"Group {group} deleted.");
            Logged.DeleteGroup(group);
        }

        #endregion

        #endregion
    }
}
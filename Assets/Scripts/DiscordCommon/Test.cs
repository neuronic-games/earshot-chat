using System;
using System.Collections.Generic;
using Discord;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordCommon
{
    public class Test : MonoBehaviour
    {
        public Button voiceSettingsButton;
        public Button addLobbyButton;
        public RectTransform contentContainer;
        public LobbyTest toInst;

        private Discord.Discord _discord;
        private LobbyManager _manager;
        
        public void Awake()
        {
            addLobbyButton.onClick.AddListener(AddLobby);
            voiceSettingsButton.onClick.AddListener(VoiceSettings);
            var controller = FindObjectOfType<DiscordController>();
            if (DiscordApp.Discord != null)
            {
                _discord = DiscordApp.Discord;
            }
            else
            {
                controller.onCreated.AddListener(FetchDiscord);
            }

            void FetchDiscord(Discord.Discord discord)
            {
                _discord = discord;
            }
        }

        private void VoiceSettings()
        {
            _discord.GetOverlayManager().OpenVoiceSettings(null);
        }

        public void AddLobby()
        {
            if (_manager == null)
            {
                if (_discord == null)
                {
                    Debug.Log($"Discord not initiated yet.");
                    return;
                }

                _manager = _discord.GetLobbyManager();
                _manager.OnLobbyUpdate += OnLobbyUpdate;
                _manager.OnLobbyDelete += OnLobbyDelete;
                _manager.OnLobbyMessage += OnLobbyMessage;
            }
            var inst = Instantiate(toInst, contentContainer);
            inst.Setup(_manager);
        }

        private void OnLobbyMessage(long lobbyid, long userid, byte[] data)
        {
            Debug.Log($"Message received in lobby {lobbyid} from user {userid}. \nMessage: {data}");
        }

        private void OnLobbyDelete(long lobbyid, uint reason)
        {
            Debug.Log($"Lobby {lobbyid} deleted. Reason: {reason}"); 
        }

        private void OnLobbyUpdate(long lobbyid)
        {
            Debug.Log($"Lobby {lobbyid} updated.");
        }
    }
}
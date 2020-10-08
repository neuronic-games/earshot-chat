using System.Globalization;
using Discord;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordCommon
{
    public class Test : MonoBehaviour
    {
        /*public Button         voiceSettingsButton;
        public Button         addLobbyButton;
        public Button         joinLobbyButton;
        public TMP_InputField lobbyIdInput;
        public TMP_InputField lobbySecretInput;
        public RectTransform  contentContainer;
        public LobbyTest      toInst;

        private Discord.Discord _discord;
        private LobbyManager    _manager;

        public void Awake()
        {
            addLobbyButton.onClick.AddListener(AddLobby);
            voiceSettingsButton.onClick.AddListener(VoiceSettings);
            joinLobbyButton.onClick.AddListener(TryJoinLobby);
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
            if (Validate()) return;

            var inst = Instantiate(toInst, contentContainer);
            inst.Setup(_manager);
        }

        private bool Validate()
        {
            if (_manager == null)
            {
                if (_discord == null)
                {
                    Debug.Log($"Discord not initiated yet.");
                    return true;
                }

                _manager                =  _discord.GetLobbyManager();
                _manager.OnLobbyUpdate  += OnLobbyUpdate;
                _manager.OnLobbyDelete  += OnLobbyDelete;
                _manager.OnLobbyMessage += OnLobbyMessage;
            }

            return false;
        }

        public void TryJoinLobby()
        {
            if (Validate()) return;
            string lobbyId     = lobbyIdInput.text;
            var    lobbySecret = lobbySecretInput.text;
            if (string.IsNullOrEmpty(lobbyId) || string.IsNullOrEmpty(lobbySecret)) return;
            if (!long.TryParse(lobbyId, out long lobby))
            {
                return;
            }

            var inst = Instantiate(toInst, contentContainer);

            inst.Manager = _manager;
            _manager.ConnectLobby(lobby, lobbySecret, inst.OnLobbyCreate);
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
        }*/
    }
}
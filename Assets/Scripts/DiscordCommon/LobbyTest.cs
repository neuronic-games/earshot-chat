using System;
using Discord;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DiscordCommon
{
    public class LobbyTest : MonoBehaviour, IPointerDownHandler
    {
        #region Lobby Creation
        
        public TextMeshProUGUI lobbyName;
        public TextMeshProUGUI lobbyCapacity;

        [NonSerialized]
        public Lobby lobby;

        private long         _id;
        private LobbyManager _manager;

        public void Setup(LobbyManager manager)
        {
            _manager = manager;

            SetupDiscordLobby();
        }

        private void SetupDiscordLobby()
        {
            var lobbyTransaction = _manager.GetLobbyCreateTransaction();
            lobbyTransaction.SetCapacity(5);
            lobbyTransaction.SetType(LobbyType.Public);
            
            _manager.CreateLobby(lobbyTransaction, OnLobbyCreate);
        }

        private void OnLobbyCreate(Result result, ref Lobby lobby)
        {
            if (result == Result.Ok)
            {
                this.lobby = lobby;
                Debug.Log("Successfully created lobby.");

                RefreshUI();
            }
            else
            {
                Debug.Log($"Error during creating lobby. Result: {result}.");
            }
        }

        public void RefreshUI()
        {
            lobbyName.text     = lobby.Id.ToString();
            lobbyCapacity.text = lobby.Capacity.ToString();
        }
        
        #endregion
        
        #region Voice

        public GameObject voiceIndicator;

        private bool voiceConnected = false;

        private bool loading = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (voiceConnected)
            {
                DisconnectVoice();
            }
            else
            {
                ConnectVoice();
            }
        }

        public void DisconnectVoice()
        {
            if (!voiceConnected || loading) return;
            loading = true;
            _manager.ConnectVoice(lobby.Id, OnVoiceConnected);
            
            void OnVoiceConnected(Result result)
            {
                loading = false;
                if (result == Result.Ok)
                {
                    voiceConnected = false;
                }
                voiceIndicator.SetActive(voiceConnected);
                Debug.Log($"Lobby {lobby.Id}. Voice Connected: {voiceConnected}");
            }
        }


        public void ConnectVoice()
        {
            if (voiceConnected || loading) return;
            loading = true;
            _manager.DisconnectVoice(lobby.Id, OnVoiceDisconnect);
            
            void OnVoiceDisconnect(Result result)
            {
                loading = false;
                if (result == Result.Ok)
                {
                    voiceConnected = true;
                }
                voiceIndicator.SetActive(voiceConnected);
                Debug.Log($"Lobby {lobby.Id}. Voice Connected: {voiceConnected}");
            }
        }

        #endregion
    }
}
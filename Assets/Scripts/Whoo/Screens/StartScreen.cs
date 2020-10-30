using System;
using Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Screen = UI.Screens.Screen;

namespace Whoo
{
    public class StartScreen : Screen
    {
        #region Serializable

        [Header("Joining")]
        public Button joinButton;

        public Button makeRoomButton;

        public TMP_InputField lobbyIdInput;
        public TMP_InputField lobbySecretInput;

        #endregion
        
        public void Awake()
        {
            joinButton.onClick.AddListener(JoinRoom);
            makeRoomButton.onClick.AddListener(MakeRoom);
        }

        #region Screen

        public override void Setup()
        {
            lobbyIdInput.text     = string.Empty;
            lobbySecretInput.text = string.Empty;
        }

        public override void Refresh()
        {
            
        }

        #endregion

        private bool _loading = false;

        #region UI Methods

        public void MakeRoom()
        {
            if (_loading) return;
            var app = AppLayer.AppLayer.Get();
            if (app.CanCreateGroup)
            {
                _loading = true;
                app.CreateNewGroup(5, false, group =>
                {
                    _loading = false;
                    if (group == null) return;
                    Build.ToRoomScreen(@group, true);
                });
            }
            else
            {
                Debug.Log($"Can't create any more groups.");
            }
        }

        public void JoinRoom()
        {
            if (_loading) return;
            _loading = true;
            if (!ExtractLobbyIdFromInput(lobbyIdInput.text, out long lobbyId)) return;

            if (!ExtractSecretFromJoinInput(lobbySecretInput.text, out string secret))
            {
                Dialog.Get().RequestInfo("Invalid Input", "Secret is incorrect.", DialogStyle.Error, null);
                return;
            }

            var app = AppLayer.AppLayer.Get();
            if (app.CanJoinGroup)
            {
                app.JoinGroup(lobbyId, secret, group =>
                {
                    _loading = false;
                    if (group == null)
                    {
                        Dialog.Get().
                               RequestInfo("Failed to Join", "Either Room Id or Password is incorrect.",
                                   DialogStyle.Error,        null);
                        return;
                    } //should be logged by logging service

                    Whoo.Build.ToRoomScreen(group, false);
                });
            }
            else
            {
                Debug.Log($"Can't join any more groups.");
            }
        }

        private bool ExtractLobbyIdFromInput(string text, out long lobbyId)
        {
            bool success = long.TryParse(text, out lobbyId);
            if (!success)
            {
                Dialog.Get().
                       RequestInfo("Invalid Input", "Lobby Id is supposed to be numeric.", DialogStyle.Error, null);
            }

            return success;
        }

        private bool ExtractSecretFromJoinInput(string input, out string secret)
        {
            bool success = input.Length > 0;
            if (!success)
            {
                Dialog.Get().RequestInfo("Invalid Input", "Secret is empty.", DialogStyle.Error, null);
            }

            secret = input.Trim(' ');
            return success;
        }

        #endregion
    }
}
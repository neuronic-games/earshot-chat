using System;
using Discord;
using UI.Screens;
using UnityEngine;
using UnityEngine.EventSystems;
using Whoo.Screens;
using Whoo.Views;

namespace Whoo
{
    [CreateAssetMenu(menuName = "Whoo/Settings", order = 0)]
    public class WhooSettings : ScriptableSingleton<WhooSettings>
    {
        [Header("Discord")]
        public long discordAppId;

        public CreateFlags discordCreateFlags;

        [Header("Tester")]
        public TesterInfo testerInfo;

        [Header("Screens")]
        public AuthenticationScreen authScreen;

        public RoomScreen roomScreen;

        public StartScreen startScreen;

        public WaitingLobbyScreen waitingScreen;

        public GameObject fadeOut;

        [Header("Misc Values")]
        public float waitingLobbyRefreshInterval = 5.0f;

        public float daemonUpdateInterval = 10.0f;

        [Header("Templates")]
        public TableView tableView;

        [Header("Fade Settings")]
        [SerializeField]
        private float startAmount;

        [SerializeField]
        private float endAmount;

        [SerializeField]
        private float duration;

        public FadeOut.FadeSettings DefaultFadeSettings => new FadeOut.FadeSettings()
        {
            StartAmount  = startAmount,
            EndAmount    = endAmount,
            FadeDuration = duration,
            EndAction    = FadeOut.EndAction.DisableObject
        };

        [Header("Default Sprites")]
        public Sprite transparent;

        public TableFlowerMenuSprites tableFMenuSprites;

        [Header("Avatar Builder")]
        public AvatarSprites maleSprites;

        public AvatarSprites femaleSprites;
    }

    [Serializable]
    public class TesterInfo
    {
        public string profileId = "5fc0e867f07e96041b1275e5";
    }

    [Serializable]
    public class TableFlowerMenuSprites
    {
        public Sprite showInfo;
        public Sprite eavesdrop;
        public Sprite startGroupChat;
        public Sprite loadFolder;
        public Sprite toggleTags;
        public Sprite toggleStopwatch;
        public Sprite clearTable;
    }

    [Serializable]
    public class AvatarSprites
    {
        public Sprite   baseSprite;
        public Sprite[] hair;
        public Sprite[] faceA;
        public Sprite[] faceB;
        public Sprite[] torso;
    }
}
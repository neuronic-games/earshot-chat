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

        [Header("Screens")]
        public RoomScreen roomScreen;

        public StartScreen startScreen;

        public FadeOut fadeOut;

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
            startAmount  = startAmount,
            endAmount    = endAmount,
            fadeDuration = duration,
            endAction    = FadeOut.EndAction.DisableObject
        };

        [Header("Tables")]
        public TableDisplayProperties[] defaultTablesWhenCreate;

        [Header("Default Sprites")]
        public TableFlowerMenuSprites tableFMenuSprites;

        [Header("Avatar Builder")]
        public AvatarSprites maleSprites;

        public AvatarSprites femaleSprites;
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

    [Serializable]
    public class TableDisplayProperties
    {
        public Rect rect;
        public int  seats;
    }
}
using System;
using Discord;
using UI.Screens;
using UnityEngine;
using Whoo.Views;

namespace Whoo
{
    [CreateAssetMenu(menuName = "Whoo/Settings", order = 0)]
    public class WhooSettings : ScriptableSingleton<WhooSettings>
    {
        [Header("Discord")]
        public long        discordAppId;
        public CreateFlags discordCreateFlags;

        [Header("Screens")]
        public RoomScreen roomScreen;

        public StartScreen startScreen;

        public FadeOut fadeOut;

        [Header("Templates")]
        public TableView tableView;

        [Header("Fade Settings")]
        [SerializeField] private float startAmount;
        [SerializeField] private float endAmount;
        [SerializeField] private float duration;
        
        public FadeOut.FadeSettings DefaultFadeSettings => new FadeOut.FadeSettings()
        {
            startAmount = startAmount,
            endAmount = endAmount,
            fadeDuration = duration,
            endAction = FadeOut.EndAction.DisableObject
        };

        [Header("Tables")]
        public TableDisplayProperties[] defaultTablesWhenCreate;
    }

    [Serializable]
    public class TableDisplayProperties
    {
        public Rect rect;
        public int  seats;
    }
}
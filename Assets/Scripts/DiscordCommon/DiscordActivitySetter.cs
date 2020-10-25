using Discord;
using DiscordAppLayer;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DiscordCommon
{
    public class DiscordActivitySetter : MonoBehaviour
    {
        /// <summary>
        /// Whether to set activity whenever this gameobject is enabled.
        /// Useful for putting this on various game screens to automatically change activity based on which
        /// screen was last displayed.
        /// </summary>
        [SerializeField]
        private bool setOnEnable = true;

        [Header("If null, Clear."), SerializeField]
        private SerializedDiscordActivity activity = null;

        public void OnEnable()
        {
            if (setOnEnable)
            {
                SetActivity();
            }
        }

        public void SetActivity()
        {
            if (!DiscordApp.GetDiscordApp(out DiscordApp app)) return;

            var manager = app.ActivityManager;
            if (activity != null)
                manager.UpdateActivity(activity.GetActivity(), OnActivitySet);
            else manager.ClearActivity((result => { Debug.Log($"Clear Activity result: {result}"); }));
        }

        public static Discord.Discord GetManager()
        {
            return new Discord.Discord();
        }

        private static void OnActivitySet(Result result)
        {
            if (result != Result.Ok)
            {
                Debug.LogWarning($"Failed to set activity. Reason: {result}.");
            }
            else
            {
                Debug.Log($"Successfully set activity.");
            }
        }
    }
}
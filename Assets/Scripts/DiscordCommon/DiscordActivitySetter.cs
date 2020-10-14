using Discord;
using DiscordAppLayer;
using UnityEngine;

namespace DiscordCommon
{
    public class DiscordActivitySetter : MonoBehaviour
    {
        [SerializeField] private bool setOnEnable = true;

        [SerializeField] private SerializedDiscordActivity activity;

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
            manager.UpdateActivity(activity.GetActivity(), OnActivitySet);
        }

        private static void OnActivitySet(Result result)
        {
            if (result != Result.Ok)
            {
                Debug.LogWarning($"Failed to set activity. Reason: {result}.");
            }
        }
    }
}
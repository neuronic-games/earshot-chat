using AppLayer.Callbacks;
using Discord;

namespace DiscordAppLayer
{
    /// <summary>
    /// Discord specific activity/rich presence callbacks.
    /// </summary>
    public interface IActivityCallbacks : ICallbacks
    {
        void OnActivityJoin(ActivityManager        activityManager, string   secret);
        void OnActivityJoinRequest(ActivityManager activityManager, ref User user);
        void OnActivitySpectate(ActivityManager    activityManager, string   secret);

        void OnActivityInvite(ActivityManager activityManager, ActivityActionType type, ref User user,
            ref Activity                      activity);
    }
}
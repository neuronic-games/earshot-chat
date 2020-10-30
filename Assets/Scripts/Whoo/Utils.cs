using System;
using System.Collections;
using AppLayer.NetworkGroups;
using UnityEngine;

namespace Whoo
{
    public static class Utils
    {
        #region Time

        public static TimeSpan EpochToNowSpan => (DateTime.UtcNow - new DateTime(1970, 1, 1));

        public static double EpochToNowSeconds => EpochToNowSpan.TotalSeconds;

        #endregion

        #region Coroutines

        #endregion

        public static void DelayedAction(Action action, float seconds)
        {
            CoroutineSingleton.StartCor(_DelayedAction(action, seconds));
        }

        private static IEnumerator _DelayedAction(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        #region Whoo-specific

        public static void SetSitting(this IUser user, bool isSitting)
        {
            string value = isSitting ? Constants.True : null;
            user.SetOrDeleteCustomProperty(Constants.Sitting, value);
        }

        public static bool IsSitting(this IUser user)
        {
            return user.CustomProperties.TryGetValue(Constants.Sitting, out var value) && value == Constants.True;
        }

        public static string GetTableMetaDataKey(int i) => $"Table{i}";

        #endregion
    }
}
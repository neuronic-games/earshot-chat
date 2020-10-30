using System;
using System.Collections;
using AppLayer.NetworkGroups;
using UnityEngine;
using UnityEngine.EventSystems;

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

        #region PointerEventData

        public static bool LeftClick(this   PointerEventData evt) => evt.button == PointerEventData.InputButton.Left;
        public static bool RightClick(this  PointerEventData evt) => evt.button == PointerEventData.InputButton.Right;
        public static bool MiddleClick(this PointerEventData evt) => evt.button == PointerEventData.InputButton.Middle;

        #endregion
    }
}
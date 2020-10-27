using System;
using System.Collections;
using HarshCommon.Patterns.Singleton;
using UnityEngine;

namespace Whoo
{
    public static class Utils
    {
        public static TimeSpan EpochToNowSpan => (DateTime.UtcNow - new DateTime(1970, 1, 1));

        public static double EpochToNowSeconds => EpochToNowSpan.TotalSeconds;

        public static void DelayedAction(Action action, float seconds)
        {
            CoroutineSingleton.StartCor(_DelayedAction(action, seconds));
        }

        private static IEnumerator _DelayedAction(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}
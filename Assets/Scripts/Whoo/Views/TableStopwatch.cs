using System;
using DiscordAppLayer;
using UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Whoo.Views
{
    public class TableStopwatch : MonoBehaviour
    {
        [Header("References")]
        public TableView view;

        public Timer timer;

        public bool IsRunning { get; private set; }

        public bool CanStop
        {
            get
            {
                bool playerLeft = true;
                for (var i = 0; i < view.Table.Group.Members.Count; i++)
                {
                    playerLeft &= (view.Table.Group.Members[i].UniqueId != StartedBy);
                }

                bool localPlayer = StartedBy == view.Group.LocalUser.UniqueId;

                return localPlayer || playerLeft;
            }
        }

        public int    StartTime { get; private set; }
        public string StartedBy { get; private set; }

        public void Awake()
        {
            _Stop();

            Transform up = transform.parent;
            while (up != null && view == null)
            {
                view = up.GetComponent<TableView>();
                up   = up.parent;
            }

            Assert.IsNotNull(view);

            view.onPropertiesUpdate.AddListener(CheckStopwatch);
        }

        public void CheckStopwatch()
        {
            string value = string.Empty;
            try
            {
                if (view.Table.Group.CustomProperties.TryGetValue(Constants.Stopwatch, out value))
                {
                    string[] values = value.Split(',');
                    StartedBy = values[0];
                    StartTime = int.Parse(values[1]);
                    IsRunning = true;

                    timer.Set(StartTime, Timer.TimerType.Stopwatch);

                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Unable to parse custom property'{value}'.\n{ex}");
            }

            _Stop();
        }

        private void _Stop()
        {
            IsRunning = false;
            timer.Stop();
            timer.Clear();
        }

        public void StartStopwatch()
        {
            view.Table.Group.SetOrDeleteCustomProperty(Constants.Stopwatch,
                $"{view.Group.LocalUser.UniqueId},{(int) Utils.EpochToNowSeconds}");
        }

        public void StopStopwatch()
        {
            view.Table.Group.SetOrDeleteCustomProperty(Constants.Stopwatch, null);
        }
    }
}
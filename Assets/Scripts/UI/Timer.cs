using System;
using TMPro;
using UnityEngine;
using Whoo;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        public enum TimerType
        {
            Countdown,
            Stopwatch
        }

        [SerializeField]
        private TextMeshProUGUI text;

        public string timeFormat = "{0}h {1}m {2}s";

        public TimerType timerType;

        [SerializeField]
        private int timestamp;

        private bool  _running    = false;
        private float accumulator = 0.0f;

        public void Set(int stamp, TimerType type)
        {
            gameObject.SetActive(true);
            timestamp = stamp;
            timerType = type;

            accumulator = 0.0f;
            _running    = true;

            text.text = string.Empty;
        }

        public void Stop()
        {
            _running = false;
        }

        public void Clear()
        {
            text.text = string.Empty;
        }

        public void Update()
        {
            if (!_running) return;
            accumulator += Time.deltaTime;
            while (accumulator >= 1.0f)
            {
                accumulator -= 1;
                TimeSpan span = default;
                if (timerType == TimerType.Countdown)
                {
                    span = TimeSpan.FromSeconds(timestamp - Utils.EpochToNowSeconds);
                }
                else
                {
                    span = TimeSpan.FromSeconds(Utils.EpochToNowSeconds - timestamp);
                }

                text.SetText(timeFormat, span.Hours, span.Minutes, span.Seconds);
            }
        }
    }
}
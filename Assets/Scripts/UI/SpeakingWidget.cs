using System;
using UnityEngine;

namespace UI
{
    public class SpeakingWidget : MonoBehaviour
    {
        public GameObject speakingIcon;

        public void Awake()
        {
            SetSpeaking(false);
        }

        public void SetSpeaking(bool speaking)
        {
            speakingIcon.SetActive(speaking);
        }
    }
}
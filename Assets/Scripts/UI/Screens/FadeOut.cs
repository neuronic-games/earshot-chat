using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Screens
{
    public class FadeOut : Screen<FadeOut.FadeSettings>
    {
        [SerializeField]
        private CanvasGroup group = null;

        [SerializeField]
        private float duration = 1;

        [SerializeField, Range(0, 1)]
        private float startAmount = 1;

        [SerializeField, Range(0, 1)]
        private float endAmount = 0.2f;

        [SerializeField]
        public bool fadeOnEnable = false;

        [SerializeField]
        private bool interactableOnEnable = true;

        [SerializeField]
        private bool blockRaycastsOnEnable = true;


        [Flags]
        public enum EndAction
        {
            DisableObject       = 1 << 0,
            DisableGroupObject  = 1 << 1,
            UnInteractableGroup = 1 << 2,
            InvokeEvent         = 1 << 3
        }

        [SerializeField]
        private EndAction endAction = EndAction.DisableObject;

        [SerializeField]
        private UnityEvent invokeAction = null;

        private float _fadeStart = 0.0f;
        private bool  _isFading  = false;

        private FadeSettings _currentSettings;

        public void Awake()
        {
            SetupCurrentSettings();
        }

        public void OnEnable()
        {
            if(fadeOnEnable) StartFadeOut();
        }

        public void Update()
        {
            FadeUpdate();
        }

        private void StartFadeOut()
        {
            _isFading  = true;
            _fadeStart = Time.time;

            SetupCurrentSettings();

            if (_currentSettings.startAmount != null) @group.alpha = _currentSettings.startAmount.Value;
            @group.blocksRaycasts = interactableOnEnable;
            @group.interactable   = blockRaycastsOnEnable;
        }

        private void SetupCurrentSettings()
        {
            _currentSettings.startAmount  = _currentSettings.startAmount  ?? startAmount;
            _currentSettings.endAmount    = _currentSettings.endAmount    ?? endAmount;
            _currentSettings.fadeDuration = _currentSettings.fadeDuration ?? duration;
            _currentSettings.endAction    = _currentSettings.endAction    ?? endAction;
        }

        private void FadeUpdate()
        {
            if (!_isFading) return;
            float animFactor = (Time.time - _fadeStart) / (_currentSettings.fadeDuration.Value);
            if (animFactor >= 1.0f)
            {
                TakeAction();
                Hide();
            }

            @group.alpha = Mathf.Lerp(_currentSettings.startAmount.Value, _currentSettings.endAmount.Value,
                Mathf.Clamp01(animFactor));
        }

        private void TakeAction()
        {
            _isFading = false;
            var _endAction = _currentSettings.endAction.Value;
            if (_endAction.HasFlag(EndAction.DisableObject))
            {
                gameObject.SetActive(false);
            }

            if (_endAction.HasFlag(EndAction.DisableGroupObject))
            {
                @group.gameObject.SetActive(false);
            }

            if (_endAction.HasFlag(EndAction.InvokeEvent))
            {
                invokeAction.Invoke();
            }

            if (_endAction.HasFlag(EndAction.UnInteractableGroup))
            {
                @group.blocksRaycasts = false;
                @group.interactable   = false;
            }
            _currentSettings.onEnd?.Invoke();
            _currentSettings.onEnd = null;
        }

        #region Screen Settings

        [Serializable]
        public struct FadeSettings : IScreenSettings
        {
            public float?     startAmount;
            public float?     endAmount;
            public float?     fadeDuration;
            public EndAction? endAction;
            public Action onEnd;
        }

        #endregion

        #region IScreen

        public override void Refresh()
        {
            StartFadeOut();
        }

        public override void Setup(ref FadeSettings settings)
        {
            _isFading = false;
            
            _currentSettings = settings;

            SetupCurrentSettings();
        }

        #endregion
    }
}
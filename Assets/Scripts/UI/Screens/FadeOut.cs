using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("endAction")]
        [SerializeField]
        private EndAction defaultEndAction = EndAction.DisableObject;

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
            if (fadeOnEnable) StartFadeOut();
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

            if (_currentSettings.StartAmount != null) @group.alpha = _currentSettings.StartAmount.Value;
            @group.blocksRaycasts = interactableOnEnable;
            @group.interactable   = blockRaycastsOnEnable;
        }

        private void SetupCurrentSettings()
        {
            _currentSettings.StartAmount  = _currentSettings.StartAmount  ?? startAmount;
            _currentSettings.EndAmount    = _currentSettings.EndAmount    ?? endAmount;
            _currentSettings.FadeDuration = _currentSettings.FadeDuration ?? duration;
            _currentSettings.EndAction    = _currentSettings.EndAction    ?? defaultEndAction;
        }

        private void FadeUpdate()
        {
            if (!_isFading) return;
            float animFactor = (Time.time - _fadeStart) / (_currentSettings.FadeDuration ?? 1f);
            if (animFactor >= 1.0f)
            {
                TakeAction();
                Hide().Forget();
            }

            @group.alpha = Mathf.Lerp(_currentSettings.StartAmount ?? 1f, _currentSettings.EndAmount ?? 0f,
                Mathf.Clamp01(animFactor));
        }

        private void TakeAction()
        {
            _isFading = false;
            var _endAction = _currentSettings.EndAction ?? defaultEndAction;
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

            _currentSettings.OnEnd?.Invoke();
            _currentSettings.OnEnd = null;
        }

        #region Screen Settings

        [Serializable]
        public struct FadeSettings : IScreenSettings
        {
            public float?     StartAmount;
            public float?     EndAmount;
            public float?     FadeDuration;
            public EndAction? EndAction;
            public Action     OnEnd;
        }

        #endregion

        #region IScreen

        public override async UniTask Setup(FadeSettings settings)
        {
            _isFading = false;

            _currentSettings = settings;

            SetupCurrentSettings();

            await UniTask.CompletedTask; // suppresses warning
        }

        #endregion
    }
}
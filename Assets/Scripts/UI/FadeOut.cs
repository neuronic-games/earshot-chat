using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class FadeOut : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup group;

        [SerializeField]
        private float duration;

        [SerializeField, Range(0, 1)]
        private float startAmount;

        [SerializeField, Range(0, 1)]
        private float endAmount;

        [SerializeField]
        private bool interactableOnEnable = true;

        [SerializeField]
        private bool blockRaycastsOnEnable = true;

        [SerializeField]
        private bool fadeOutOnEnable = true;

        [Flags]
        public enum EndAction
        {
            DisableObject       = 1 << 0,
            DisableGroupObject           = 1 << 1,
            UnInteractableGroup = 1 << 2,
            InvokeEvent         = 1 << 3
        }

        [SerializeField]
        private EndAction endAction;

        [SerializeField]
        private UnityEvent invokeAction;

        private float _fadeStart = 0.0f;
        private bool  _isFading  = false;

        public void OnEnable()
        {
            if (!fadeOutOnEnable) return;
            StartFadeOut();
        }

        public void Update()
        {
            FadeUpdate();
        }

        private void StartFadeOut()
        {
            _isFading  = true;
            _fadeStart = Time.time;

            @group.alpha          = startAmount;
            @group.blocksRaycasts = interactableOnEnable;
            @group.interactable   = blockRaycastsOnEnable;
        }

        private void FadeUpdate()
        {
            if (!_isFading) return;
            float animFactor = (Time.time - _fadeStart) / (duration);
            if (animFactor >= 1.0f)
            {
                TakeAction();
            }

            @group.alpha = Mathf.Lerp(startAmount, endAmount, Mathf.Clamp01(animFactor));
        }

        private void TakeAction()
        {
            _isFading = false;
            if (endAction.HasFlag(EndAction.DisableObject))
            {
                gameObject.SetActive(false);
            }
            if (endAction.HasFlag(EndAction.DisableGroupObject))
            {
                @group.gameObject.SetActive(false);
            }
            if (endAction.HasFlag(EndAction.InvokeEvent))
            {
                invokeAction.Invoke();
            }
            if (endAction.HasFlag(EndAction.UnInteractableGroup))
            {
                @group.blocksRaycasts = false;
                @group.interactable   = false;
            }
        }
    }
}
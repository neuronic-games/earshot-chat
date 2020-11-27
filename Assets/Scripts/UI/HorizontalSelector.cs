using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Horizontal Selector script. Listen to the onSelectorChanged script and use the new index to figure out next action.
    /// You can iterate over this by looping [0, ItemCount)
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class HorizontalSelector : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI labelText;

        public Image labelImage;

        public Button left;
        public Button right;

        [Header("Settings")]
        public bool loop;

        public bool invokeOnStart;

        public int defaultIndex = 0;

        public int minimumItemsForInteractableButtons = 1;

        public List<Item> items;

        [Header("Animations"), SerializeField]
        private Animator animator;

        public string idleClip  = "Idle";
        public string leftClip  = "Left";
        public string rightClip = "Right";

        [Header("Events")]
        public IntEvent selectorChanged;

        #region Class Def

        [Serializable]
        public class IntEvent : UnityEvent<int>
        {
        }

        [Serializable]
        public class Item
        {
            public Sprite     sprite         = null;
            public string     label          = string.Empty;
            public UnityEvent onValueChanged = new UnityEvent();
        }

        #endregion

        #region Private Members

        private List<Item> _runtimeItems = new List<Item>();

        private int _currentIndex = -1;
        public  int ItemCount => _runtimeItems.Count;

        public int CurrentIndex => _currentIndex;

        public bool HasItems => ItemCount > 0;

        #endregion

        #region Unity Callbacks

        public void Awake()
        {
            _runtimeItems.AddRange(items);
            if (_currentIndex == -1)
                _currentIndex = defaultIndex;

            left.onClick.AddListener(SelectorLeft);
            right.onClick.AddListener(SelectorRight);
        }

        public void Start()
        {
            if (invokeOnStart)
            {
                InvokeEvents();
            }

            RefreshUI();
        }

        #endregion

        public void AddItem(Item item)
        {
            int index = _runtimeItems.IndexOf(item);
            if (index == -1)
            {
                _runtimeItems.Add(item);
                if (ItemCount == 1) SelectorRight();
            }
        }

        public bool RemoveItem(Item item)
        {
            int index = _runtimeItems.IndexOf(item);
            if (index == -1) return false;

            _runtimeItems.Remove(item);

            if (index <= _currentIndex)
            {
                MoveLeftWithoutNotify();
                MoveRightWithoutNotify();

                if (index == _currentIndex)
                {
                    RefreshUI();
                    InvokeEvents();
                }
            }

            return true;
        }

        public void Clear()
        {
            _runtimeItems.Clear();
            _currentIndex = -1;
            RefreshUI();
        }

        public Item GetAt(int index) => (index >= 0 && index < ItemCount) ? _runtimeItems[index] : null;

        public bool Select(int index)
        {
            return Select(GetAt(index));
        }
        
        public bool Select(Item item)
        {
            if (item == null)
            {
                _currentIndex = -1;
                RefreshUI();
                InvokeEvents();
                return false;
            }
            
            bool success = SelectWithoutNotify(item);
            
            RefreshUI();

            if (success)
            {
                InvokeEvents();
            }

            return success;
        }

        public bool SelectWithoutNotify(Item item)
        {
            var index = _runtimeItems.IndexOf(item);
            if (index != -1)
            {
                _currentIndex = index;
                RefreshUI();
                return true;
            }

            return false;
        }

        private void RefreshUI()
        {
            bool interactable = ItemCount >= minimumItemsForInteractableButtons;
            left.interactable  = interactable && (loop || _currentIndex > 0);
            right.interactable = interactable && (loop || _currentIndex < ItemCount - 1);

            if (labelText != null)
            {
                labelText.text = HasItems ? _runtimeItems[_currentIndex].label : string.Empty;
            }

            if (labelImage != null)
            {
                labelImage.overrideSprite = HasItems ? _runtimeItems[_currentIndex].sprite : null;
            }
        }

        private void InvokeEvents()
        {
            if (_currentIndex >= 0 && _currentIndex < ItemCount)
            {
                if (HasItems)
                    _runtimeItems[_currentIndex].onValueChanged.Invoke();
                selectorChanged.Invoke(_currentIndex);
            }
        }

        private void SelectorLeft()
        {
            if (animator != null)
            {
                animator.Play(leftClip);
            }

            MoveLeftWithoutNotify();

            RefreshUI();
            InvokeEvents();
        }

        public void MoveLeftWithoutNotify()
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                if (loop) _currentIndex = ItemCount - 1;
                else _currentIndex      = 0;
            }
        }

        private void SelectorRight()
        {
            if (animator != null)
            {
                animator.Play(rightClip);
            }

            MoveRightWithoutNotify();

            RefreshUI();
            InvokeEvents();
        }

        public void MoveRightWithoutNotify()
        {
            _currentIndex++;
            if (_currentIndex >= ItemCount)
            {
                if (loop) _currentIndex = 0;
                else _currentIndex      = ItemCount - 1;
            }
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UI.FlowerMenu;
using UI.Screens;
using UnityEngine;
using UnityEngine.Events;

namespace Whoo.Views
{
    public class TableFlowerMenu : MonoBehaviour, IScreen
    {
        #region Serialized

        [Header("References")]
        [SerializeField]
        private TableView view = default;

        [SerializeField]
        private TableStopwatch watch;

        [SerializeField]
        private Animator animator = default;

        [SerializeField]
        private RectTransform container = default;

        [SerializeField]
        private MenuItem template = default;

        #endregion

        public void Awake()
        {
            if (view == null)
            {
                view = GetComponentInParent<TableView>();
            }

            if (watch == null)
            {
                watch = GetComponentInChildren<TableStopwatch>();
            }
        }

        public void OnEnable()
        {
            Setup();
        }

        #region Menu Building

        public void CleanMenu()
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void BuildMenu()
        {
            //todo -- checking if item can be created
            TableFlowerMenuSprites sprites = Whoo.Build.Settings.tableFMenuSprites;

            MakeMenuItem(sprites.showInfo,       ShowInfo);
            MakeMenuItem(sprites.eavesdrop,      Eavesdrop);
            MakeMenuItem(sprites.startGroupChat, StartGroupChat);
            MakeMenuItem(sprites.loadFolder,     LoadFolder);
            MakeMenuItem(sprites.toggleTags,     ToggleTags);

            if (watch != null && view.Table.Group != null && view.Table.Group.IsOwner)
            {
                MakeMenuItem(sprites.toggleStopwatch, ToggleStopwatch);
                if (!watch.IsCleared)
                {
                    MakeMenuItem(sprites.clearTable, watch.ClearTimer);
                }
            }
        }

        private MenuItem MakeMenuItem(Sprite sprite, UnityAction onClick)
        {
            MenuItem copy = Instantiate(template, container);
            copy.gameObject.SetActive(true);
            copy.image.sprite = sprite;
            copy.button.onClick.AddListener(onClick);
            return copy;
        }

        private void ToggleStopwatch()
        {
            if (watch.IsRunning)
            {
                if (watch.CanStop)
                    watch.StopStopwatch();
            }
            else
            {
                watch.StartStopwatch();
            }
        }

        private void ToggleTags()
        {
            //todo
        }

        private void LoadFolder()
        {
            //todo
        }

        private void StartGroupChat()
        {
            //todo
        }

        private void Eavesdrop()
        {
            //todo
        }

        private void ShowInfo()
        {
            //todo
        }

        #endregion

        #region Screen

        public bool IsDisplayed
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public void Setup()
        {
            CleanMenu();
            BuildMenu();
        }

        public void Display()
        {
            if (IsDisplayed) return;
            IsDisplayed = true;
            if (animator != null) animator.Play("In");
        }

        public void Hide()
        {
            if (!IsDisplayed) return;
            HideAsync().Forget();
        }

        public async UniTaskVoid HideAsync()
        {
            if (animator != null)
            {
                animator.Play("Out");
                await UniTask.WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Out"));
                IsDisplayed = false;
            }
        }

        public void Refresh()
        {
        }

        public void Close()
        {
        }

        #endregion
    }
}
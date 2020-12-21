﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Screens
{
    /// <summary>
    /// Empty interface for organizational porpoises.
    /// </summary>
    public interface IScreenSettings
    {
    }

    public interface IScreen
    {
        bool    IsDisplayed { get; }
        UniTask Setup();
        UniTask Display();
        UniTask Hide();
        UniTask Refresh();
        UniTask Close();
    }

    public interface IScreen<T> : IScreen where T : struct, IScreenSettings
    {
        UniTask Setup(T settings);
    }

    [Serializable]
    public struct EmptySettings : IScreenSettings
    {
    }

    public abstract class Screen<TSettings> : MonoBehaviour, IScreen<TSettings>
        where TSettings : struct, IScreenSettings
    {
        public virtual bool IsDisplayed
        {
            get => gameObject.activeSelf;
            protected set => gameObject.SetActive(value);
        }

        public virtual async UniTask Setup()
        {
        }

        public virtual async UniTask Close()
        {
            if (IsDisplayed) Hide();
            currentSettings = default;
        }

        public virtual async UniTask Setup(TSettings settings)
        {
            currentSettings = settings;
        }

        public virtual async UniTask Hide()
        {
            IsDisplayed = false;
        }

        public abstract UniTask Refresh();

        protected TSettings currentSettings = default;

        public virtual async UniTask Display()
        {
            IsDisplayed = true;
            await Refresh();
        }
    }

    public abstract class Screen : Screen<EmptySettings>
    {
        public sealed override async UniTask Setup(EmptySettings settings)
        {
            await base.Setup(settings);
        }
    }
}
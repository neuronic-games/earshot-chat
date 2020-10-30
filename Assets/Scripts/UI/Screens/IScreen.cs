using System;
using System.Collections.Generic;
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
        bool IsDisplayed { get; }
        void Setup();
        void Display();
        void Hide();
        void Refresh();
        void Close();
    }

    public interface IScreen<T> : IScreen where T : struct, IScreenSettings
    {
        void Setup(ref T settings);
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

        public virtual void Setup()
        {
        }

        public virtual void Close()
        {
            if(IsDisplayed) Hide();
            currentSettings = default;
        }

        public virtual void Setup(ref TSettings settings)
        {
            currentSettings = settings;
        }

        public virtual void Hide()
        {
            IsDisplayed = false;
        }

        public abstract void Refresh();

        protected TSettings currentSettings = default;

        public virtual void Display()
        {
            IsDisplayed = true;
            Refresh();
        }
    }

    public abstract class Screen : Screen<EmptySettings>
    {
        public sealed override void Setup(ref EmptySettings settings)
        {
            base.Setup(ref settings);
        }
    }
}
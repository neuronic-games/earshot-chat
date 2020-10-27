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
        void Display(Dictionary<string, object> values);
        void Hide();
        void Refresh();
    }

    public interface IScreen<T> : IScreen where T : struct, IScreenSettings
    {
        void Display(ref T settings);
    }

    [Serializable]
    public struct EmptySettings : IScreenSettings
    {
        
    }

    public abstract class Screen<TSettings> : MonoBehaviour, IScreen<TSettings> where TSettings : struct, IScreenSettings
    {
        public virtual bool IsDisplayed
        {
            get => gameObject.activeSelf;
            protected set => gameObject.SetActive(value);
        }
        
        public virtual void Setup()
        {
            
        }

        public virtual void Display(Dictionary<string, object> values)
        {
            IsDisplayed = true;
        }

        public virtual void Hide()
        {
            IsDisplayed = false;
        }

        public abstract void Refresh();

        protected TSettings currentSettings = default;

        public virtual void Display(ref TSettings settings)
        {
            IsDisplayed = true;
            currentSettings = settings;
        }
    }

    public abstract class Screen : Screen<EmptySettings>
    {
        public sealed override void Display(Dictionary<string, object> values)
        {
            base.Display(values);
        }

        public sealed override void Display(ref EmptySettings settings)
        {
            base.Display(ref settings);
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace UI
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        [NotifyPropertyChangedInvocator]
        protected void SetPropertyField<T>(ref T field, T newValue, [System.Runtime.CompilerServices.CallerMemberName]
            string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
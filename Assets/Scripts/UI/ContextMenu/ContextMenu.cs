using System;
using System.Collections.Generic;

namespace UI.ContextMenu
{
    public class ContextMenu
    {
        public enum EntryType
        {
            None,
            Title,
            Divider,
            Action
        }

        public struct Entry
        {
            public EntryType EntryType;
            public Action    Action;
            public string    Name;
        }

        public Entry Title = new Entry() {EntryType = EntryType.None};

        public List<Entry> Entries = new List<Entry>();

        public void RegisterAction(string name, Action action)
        {
            Entries.Add(new Entry()
            {
                Action    = action,
                Name      = name,
                EntryType = EntryType.Action
            });
        }

        public void AddDivider()
        {
            Entries.Add(new Entry()
            {
                EntryType = EntryType.Divider
            });
        }

        public void SetTitle(string title)
        {
            Title = new Entry()
            {
                EntryType = EntryType.Title,
                Name      = title
            };
        }
    }
}
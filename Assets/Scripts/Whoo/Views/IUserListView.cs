using UnityEngine;

namespace Whoo.Views
{
    //todo -- change to interface
    public interface IUserListView
    {
        string ListUniqueId { get; }

        Transform UserContentContainer { get; }
    }
}
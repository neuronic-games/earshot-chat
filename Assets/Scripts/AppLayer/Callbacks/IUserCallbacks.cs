using AppLayer.NetworkGroups;

namespace AppLayer.Callbacks
{
    public interface IUserCallbacks : ICallbacks
    {
        void OnCurrentUserUpdate(IUser localUser);
    }
}
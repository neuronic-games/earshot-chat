namespace AppLayer.Callbacks
{
    public interface IOverlayCallbacks : ICallbacks
    {
        void OnToggle(bool manager);
    }
}
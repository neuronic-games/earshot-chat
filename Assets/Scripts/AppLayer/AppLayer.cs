using ServiceLocator;

namespace AppLayer
{
    public static class AppLayer
    {
        public static IAppLayer Get()
        {
            return Locator<IAppLayer, NullAppLayer>.Get();
        }

        public static IAppLayer GetOrNull<T>() where T : IAppLayer
        {
            return Locator<IAppLayer, NullAppLayer>.GetOrNull<T>();
        }

        public static void Set(IAppLayer service)
        {
            Locator<IAppLayer, NullAppLayer>.Set(service);
        }
    }
}
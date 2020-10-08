using ServiceLocator;

namespace AppLayer
{
    public static class AppLayer
    {
        public static IAppLayer Get()
        {
            return Locator<IAppLayer, NullAppLayer>.Get();
        }

        public static void Set(IAppLayer service)
        {
            Locator<IAppLayer, NullAppLayer>.Set(service);
        }
    }
}
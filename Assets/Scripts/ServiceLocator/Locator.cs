using UnityEngine.Assertions;

namespace ServiceLocator
{
    /// <summary>
    /// Service Locactor pattern. Current implementation uses Null service on normal call.
    /// There's a special method call, GetNotNull() that asserts existence of a service.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TNullService"></typeparam>
    public static class Locator<TService, TNullService>
        where TService : class, IService where TNullService : TService, new()
    {
        private static TService     _service     = null;
        private static TNullService _nullService = new TNullService();

        public static TService Get()
        {
            if (_service == default) return _nullService;
            return _service;
        }

        public static TService GetNotNull()
        {
            Assert.IsNotNull(_service);
            return _service;
        }

        public static TService GetOrNull<T>() where T : TService
        {
            if (_service is T service) return service;
            return _nullService;
        }

        public static void Set(TService set)
        {
            _service = set;
        }
    }
}
using UnityEngine.Assertions;

namespace UI.Dialogs
{
    public static class Locator<TService, TNullService> where TService : class, IService where TNullService : TService, new()
    {
        private static TService     _service     = null;
        private static TNullService _nullService = new TNullService();

        public static TService Get()
        {
            if (_service == null) return _nullService;
            return _service;
        }

        public static TService GetNotNull()
        {
            Assert.IsNotNull(_service);
            return _service;
        }

        public static void Set(TService set)
        {
            _service = set;
        }
    }
}
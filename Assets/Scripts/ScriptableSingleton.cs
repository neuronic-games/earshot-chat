using UnityEngine;

public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
    private static T _inst;
        
    public static T Instance
    {
        get
        {
            if (_inst == null)
            {
                var singletonsFound = Resources.FindObjectsOfTypeAll<T>();
                if (singletonsFound.Length > 0)
                    _inst = singletonsFound[0];
            }
            else
            {
                return _inst;
            }
        
            if (_inst == null)
                _inst = CreateInstance<T>();
            return _inst;
        }
    }
}
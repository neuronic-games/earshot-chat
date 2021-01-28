using System.Collections;
using UnityEngine;

public class CoroutineSingleton : Singleton<CoroutineSingleton>
{
    public static Coroutine StartCor(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }
}
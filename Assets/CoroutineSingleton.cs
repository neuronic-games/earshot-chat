using System.Collections;
using HarshCommon.Patterns.Singleton;
using UnityEngine;

public class CoroutineSingleton : Singleton<CoroutineSingleton>
{
    public static Coroutine StartCor(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }
}
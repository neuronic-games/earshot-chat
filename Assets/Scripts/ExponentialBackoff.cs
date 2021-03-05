using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public struct Failable<T>
{
    public bool   HasValue;
    public T      Value;
    public string PrettyError;

    public Failable(T value)
    {
        Value       = value;
        HasValue    = true;
        PrettyError = null;
    }

    public Failable(string prettyError)
    {
        Value       = default;
        HasValue    = false;
        PrettyError = prettyError;
    }
}


/// <summary>
/// Implements an exponential backoff (0, 1, 2, 4, ... seconds) strategy using async.
/// Doesn't implement circuit breaker.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ExponentialBackoff<T>
{
    private const    int  MAX_RETRIES = 4;
    private readonly int  _maxRetries;
    private readonly bool _useUserDefinedMaxRetries;
    private          int  _retry;

    public ExponentialBackoff(int maxRetries)
    {
        _maxRetries               = maxRetries;
        _useUserDefinedMaxRetries = true;
        _retry                    = 0;
    }

    public async UniTask<Failable<T>> Try(Func<UniTask<Failable<T>>> service, bool withCancellation = false,
        CancellationTokenSource                                      src = null)
    {
        float wait = 0.5f;
        while (_retry < (_useUserDefinedMaxRetries ? _maxRetries : MAX_RETRIES))
        {
            _retry++;

            Failable<T> result    = default;
            bool        cancelled = false;
            if (withCancellation)
            {
                (cancelled, result) =
                    await service.Invoke().WithCancellation(src.Token).SuppressCancellationThrow();
                if (cancelled) return new Failable<T>();
            }
            else
            {
                result = await service.Invoke();
            }

            if (result.HasValue) return result;

            //4th try will be 7 seconds
            wait *= 2;

            var delay = UniTask.Delay(TimeSpan.FromSeconds(wait));
            if (withCancellation)
            {
                cancelled = await delay.WithCancellation(src.Token).SuppressCancellationThrow();
                if (cancelled) return new Failable<T>();
            }
            else
            {
                await delay;
            }
        }

        return new Failable<T>();
    }
}
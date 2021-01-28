using UnityEngine;
using UnityEngine.Events;

public class LifetimeEvents : MonoBehaviour
{
    public UnityEvent onAwake;
    public UnityEvent onStart;
    public UnityEvent onEnable;
    public UnityEvent onDisable;
    public UnityEvent onDestroy;

    public void Awake()
    {
        onAwake.Invoke();
    }

    private void Start()
    {
        onStart.Invoke();
    }

    private void OnEnable()
    {
        onEnable.Invoke();
    }

    private void OnDisable()
    {
        onDisable.Invoke();
    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }
}
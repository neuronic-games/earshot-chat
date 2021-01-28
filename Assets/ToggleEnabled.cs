using UnityEngine;

public class ToggleEnabled : MonoBehaviour
{
    public bool       onAwakeActive  = false;
    public bool       onEnableActive = false;
    public GameObject target;

    private void Awake()
    {
        target.SetActive(onAwakeActive);
    }

    private void OnEnable()
    {
        target.SetActive(onEnableActive);
    }

    public void Toggle()
    {
        target.SetActive(!target.activeSelf);
    }
}
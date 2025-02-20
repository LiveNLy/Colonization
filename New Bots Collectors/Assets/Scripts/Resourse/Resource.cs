using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public event Action<Resource> ReleaseResource;
    public event Action ResourceReleased;

    public void ActionAfterHit()
    {
        ReleaseResource?.Invoke(this);
        ResourceReleased?.Invoke();
    }
}
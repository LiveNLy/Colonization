using System;
using UnityEngine;

[RequireComponent(typeof(ResourceAllocator))]
public class BaseCollisionHandler : MonoBehaviour
{
    private ResourceAllocator _resourceAllocator;

    public event Action<Resource> ResourseCollecting;

    private void OnEnable()
    {
        _resourceAllocator = GetComponent<ResourceAllocator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resourse) && _resourceAllocator.OccupiedObjects.Contains(resourse))
        {
            ResourseCollecting?.Invoke(resourse);

            resourse.ActionAfterHit();
        }
    }
}